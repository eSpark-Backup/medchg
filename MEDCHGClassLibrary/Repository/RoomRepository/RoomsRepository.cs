using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Repository.RoomRepository
{
    public class RoomsRepository : IRoomRepository
    {
        private string _npgsqlDataSource;

        public string NpgsqlDataSource
        {
            get { return _npgsqlDataSource; }
            set { _npgsqlDataSource = value; }
        }

        private IConfiguration _configureOptions;

        private readonly MedChgContext _MedChgContext;
        public RoomsRepository(IConfiguration configureOptions, MedChgContext medChgContext)
        {
            _configureOptions = configureOptions;
            _MedChgContext = medChgContext;
            _npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection");
        }
        public async Task<List<Room>> GetAllRooms()
        {
            List<Room> Rooms = new List<Room>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    string sqlQuery = "SELECT * FROM \"Rooms\" where \"IsActive\"=1";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Room RoomObj = new Room
                        {
                            // Map database columns to ApplicationUser properties
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                            // Add other properties as needed
                        };
                        Rooms.Add(RoomObj);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return Rooms;
        }

        public async Task<bool>  DeleteRoom(int Id,int UpdatedById)
        {
            try
            {
                var DeleteRoom = _MedChgContext.Rooms.FirstOrDefault(x => x.Id == Id);
                if (DeleteRoom == null)
                {
                    return false;
                }
                var RelatedVisits = _MedChgContext.PatientVisitHistoryDetailLists
                                    .Where(pvhd => pvhd.RoomId == Id && pvhd.IsActive == 1)
                                    .GroupBy(pvhd => pvhd.PatientVisitHistoryId)
                                    .Select(group => group.First())
                                    .ToList();
                foreach (var item in RelatedVisits)
                {
                    var CheckPresentPatient = _MedChgContext.PatientVisitHistory.Where(pvh => pvh.Id == item.PatientVisitHistoryId).FirstOrDefault();
                    if (CheckPresentPatient.PatientCensusStatus == CensusStatus.ADMITTED)
                    {
                        return false;
                    }
                }
                DeleteRoom.IsActive = 2;
                DeleteRoom.UpdatedAt = TimeHelper.GetTexasTime();
                DeleteRoom.UpdatedById = UpdatedById;
                _MedChgContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw;  
            }
        }

        public async Task<List<string>> GetAllOccupiedRooms(int HospitalId)
        {
            List<string> occupiedRooms = new List<string>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    string sqlQuery = $@"
                                       WITH LatestRoom AS (
                                                    SELECT 
                                                        PVHDL.""RoomId"",
                                                        PVHDL.""PatientVisitHistoryId"",
                                                        ROW_NUMBER() OVER (PARTITION BY PVHDL.""PatientVisitHistoryId"" ORDER BY PVHDL.""CreatedAt"" DESC) AS rn
                                                    FROM 
                                                        ""PatientVisitHistoryDetailLists"" PVHDL
                                                    WHERE 
                                                        PVHDL.""HospitalId"" = {HospitalId}
                                                )
                                                SELECT 
                                                    LR.""RoomId"",
                                                    R.""Name"" AS RoomName,
                                                    PVH.""PatientId"",
                                                    CONCAT(P.""FirstName"", ' ', P.""MiddleName"", ' ', P.""LastName"") AS PatientName
                                                FROM 
                                                    ""PatientVisitHistory"" PVH
                                                JOIN 
                                                    ""Patients"" P ON PVH.""PatientId"" = P.""Id""
                                                JOIN 
                                                    LatestRoom LR ON PVH.""Id"" = LR.""PatientVisitHistoryId"" AND LR.rn = 1
                                                JOIN 
                                                    ""Rooms"" R ON LR.""RoomId"" = R.""Id""
                                                WHERE
                                                    PVH.""PatientCensusStatus"" = 1;";

                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {

                        int roomId = reader.IsDBNull(reader.GetOrdinal("RoomId")) ? 0 : reader.GetInt32(reader.GetOrdinal("RoomId"));
                        string patientName = reader["PatientName"].ToString();

                        // You can customize how to format the string. Here's one option:
                        string result = $"RoomId: {roomId}, PatientName: {patientName}";

                        // Add the formatted result to the list
                        occupiedRooms.Add(result);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                // Optionally, you can log the exception (e.g., using a logger) and then rethrow or handle it
                throw;
            }

            return occupiedRooms;
        }

    }
}
