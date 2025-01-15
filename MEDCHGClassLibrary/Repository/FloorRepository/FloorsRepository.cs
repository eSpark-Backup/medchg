using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Services;
using MEDCHGClassLibrary.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Repository.FloorRepository
{
    public class FloorsRepository : IFloorRepository
    {
        private string _npgsqlDataSource;
        public string NpgsqlDataSource
        {
            get { return _npgsqlDataSource; }
            set { _npgsqlDataSource = value; }
        }
        private IConfiguration _configureOptions;
        private readonly MedChgContext _MedChgContext;
        private readonly ResponseMessage ResponseMessage;
        public FloorsRepository(IConfiguration configureOptions, MedChgContext medChgContext, ResponseMessage responseMessage)
        {
            _configureOptions = configureOptions;
            _MedChgContext = medChgContext;
            _npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection");
            ResponseMessage = responseMessage;
        }
        public async Task<List<Floor>> GetAllFloors()
        {
            List<Floor> Floors = new List<Floor>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    string sqlQuery = "SELECT * FROM \"Floors\" where \"IsActive\"=1";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Floor FloorObj = new Floor
                        {
                            // Map database columns to ApplicationUser properties
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            BuildingId = reader.GetInt32(reader.GetOrdinal("BuildingId")),
                            RoomCode = !reader.IsDBNull(reader.GetOrdinal("RoomCode")) ? reader.GetString(reader.GetOrdinal("RoomCode")) : ""
                            // Add other properties as needed
                        };
                        Floors.Add(FloorObj);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return Floors;
        }

        public async Task<ResponseMessage> DeleteFloor(int FloorId, int UpdatedById)
        {
            try
            {
                var DeleteFloor = await _MedChgContext.Floors.FirstOrDefaultAsync(x => x.Id == FloorId);
                if (DeleteFloor == null)
                {
                    ResponseMessage.Success = false;
                    ResponseMessage.Message = "Floor Not Found!";
                    ResponseMessage.ErrorCode = "FLOOR_NOT_FOUND_IN_DB";
                    return ResponseMessage;
                }
                var RelatedVisits = _MedChgContext.PatientVisitHistoryDetailLists
                                    .Where(pvhd => pvhd.FloorId == FloorId && pvhd.IsActive == 1)
                                    .GroupBy(pvhd => pvhd.PatientVisitHistoryId)
                                    .Select(group => group.First())
                                    .ToList();
                foreach (var item in RelatedVisits)
                {
                    var CheckPresentPatient = _MedChgContext.PatientVisitHistory.Where(pvh => pvh.Id == item.PatientVisitHistoryId).FirstOrDefault();
                    if (CheckPresentPatient.PatientCensusStatus == CensusStatus.ADMITTED)
                    {
                        ResponseMessage.Success = false;
                        ResponseMessage.Message = "There's some patient on floor!";
                        ResponseMessage.ErrorCode = "PATIENT_IN_FLOOR!";
                        return ResponseMessage;
                    }
                }
                _MedChgContext.PatientVisitHistoryDetailLists.Where(x => x.FloorId == FloorId).ExecuteUpdate
                    (setters => setters
              .SetProperty(b => b.IsActive, 2)
              .SetProperty(b => b.UpdatedAt, TimeHelper.GetTexasTime())
              .SetProperty(b => b.UpdatedById, UpdatedById)
                    );
                foreach (var item in RelatedVisits)
                {
                    _MedChgContext.PatientVisitHistory.Where(x => x.Id == item.PatientVisitHistoryId).ExecuteUpdate
                   (setters => setters
             .SetProperty(b => b.IsActive, 2)
             .SetProperty(b => b.UpdatedAt, TimeHelper.GetTexasTime())
             .SetProperty(b => b.UpdatedById, UpdatedById)
                   );
                }
                _MedChgContext.Rooms.Where(x => x.FloorId == FloorId).ExecuteUpdate
                   (setters => setters
             .SetProperty(b => b.IsActive, 2)
             .SetProperty(b => b.UpdatedAt, TimeHelper.GetTexasTime())
             .SetProperty(b => b.UpdatedById, UpdatedById)
                   );
                _MedChgContext.Floors.Remove(DeleteFloor);
                _MedChgContext.SaveChanges();
                ResponseMessage.Success = true;
                ResponseMessage.Message = "Floor deleted successfully!";
                ResponseMessage.ErrorCode = "FLOOR_DELETED";
                return ResponseMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<ResponseMessage> UpdateFloor(Floor model)
        {
            try
            {
                using (var context = new MedChgContext())
                {
                    var floor = await _MedChgContext.Floors.FindAsync(model.Id);
                    if (floor == null)
                    {
                        return new ResponseMessage { Success = false, ErrorCode = "404", Message = "Floor updated successfully" };
                    }
                    floor.RoomCode = model.RoomCode;
                    floor.RoomRange = model.RoomRange;
                    floor.RoomRange1 = model.RoomRange1;
                    floor.RoomRange2 = model.RoomRange2;
                    floor.UpdatedAt = model.UpdatedAt;
                    floor.UpdatedById = model.UpdatedById;
                    if (model.Rooms is not null)
                    {
                        foreach (var item in model.Rooms)
                        {
                            var newRoom = new Room
                            {
                                Name = item.Name,
                                FloorId = floor.Id,
                                IsActive = 1,
                                CreatedAt = TimeHelper.GetTexasTime(),
                                CreatedById = model.UpdatedById
                            };
                            _MedChgContext.Rooms.Add(newRoom);
                        }
                    }
                    await _MedChgContext.SaveChangesAsync();
                    return new ResponseMessage { Success = true, ErrorCode = "200", Message = "Floor updated successfully" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new ResponseMessage { Success = false, Message = ex.ToString() };
                throw;
            }
        }

        public async Task<ResponseMessage> AddFloor(Floor model)
        {
            try
            {
                await _MedChgContext.Floors.AddAsync(model);
                await _MedChgContext.SaveChangesAsync();
                return new ResponseMessage { Success = true, ErrorCode = "200", Message = "Floor Added successfully" ,Data = model };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new ResponseMessage { Success = false, Message = ex.ToString() };
                throw;
            }
        }
    }
}
