using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Services;
using MEDCHGClassLibrary.Utilities.Helpers;
using MEDCHGClassLibrary.ViewModels.Building;
using MEDCHGClassLibrary.ViewModels.Hospital;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MEDCHGClassLibrary.Repository.BuildingRepository
{
    public class BuildingsRepository : IBuildingRepository
    {
        private string _npgsqlDataSource;

        public string NpgsqlDataSource
        {
            get { return _npgsqlDataSource; }
            set { _npgsqlDataSource = value; }
        }

        private IConfiguration _configureOptions;

        private readonly MedChgContext _MedChgContext;
        public BuildingsRepository(MedChgContext medChgContext, IConfiguration configureOptions)
        {
            _MedChgContext = medChgContext;
            _configureOptions = configureOptions;
            _npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection");
        }
        public async Task<ResponseMessage> AddBuilding(Model.Building model)
        {
            try
            {
                // Check if building name already exists
                var existingBuilding = await _MedChgContext.Buildings
                    .FirstOrDefaultAsync(b => b.Name == model.Name);

                if (existingBuilding != null)
                {
                    return new ResponseMessage
                    {
                        Success = false,
                        Message = "Building Name already exists",
                        ErrorCode = "BUILDING_NAME_EXIST",
                    };
                }

                // Add building to database
                _MedChgContext.Buildings.Add(model);
                await _MedChgContext.SaveChangesAsync();
                return new ResponseMessage
                {
                    Success = true,
                    Message = "Building added successfully",
                    Data = model
                };
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "SERVER_ERROR",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<List<Model.Building>> GetAllBuildings()
        {
            List<Model.Building> Buildings = new List<Model.Building>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"Buildings\" where \"IsActive\"=1";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Model.Building Building = new Model.Building
                        {
                            // Map database columns to ApplicationUser properties
                            Name = reader.GetString(reader.GetOrdinal("BuildingName")),
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            BuildingLocation = reader.GetString(reader.GetOrdinal("BuildingLocation")),
                            HospitalId = reader.GetInt32(reader.GetOrdinal("HospitalId"))
                            // Add other properties as needed
                        };
                        Buildings.Add(Building);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return Buildings;
        }

        public async Task<BuildingPatientListVm> GetBuildingPatients(int buildingId)
        {
            var buildingPatientList = new BuildingPatientListVm();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    var sqlQuery = $@"
                                    WITH LatestVisit AS (
                                    SELECT DISTINCT ON (PVHD.""PatientVisitHistoryId"") 
                                        PVHD.""Id"", 
                                        PVHD.""BuildingId"", 
                                        PVHD.""FloorId"", 
                                        PVHD.""RoomId"", 
                                        PVHD.""PatientVisitHistoryId"", 
                                        PVHD.""CreatedAt""  -- or another relevant timestamp column
                                    FROM public.""PatientVisitHistoryDetailLists"" AS PVHD
                                    ORDER BY PVHD.""PatientVisitHistoryId"", PVHD.""CreatedAt"" DESC  -- Use the relevant column for ordering
                                )
                                SELECT 
                                    B.""Id"" AS BuildingId,
                                    B.""BuildingName"" AS BuildingName,
                                    B.""BuildingLocation"" AS BuildingLocation,
                                    F.""Id"" AS FloorId,
                                    F.""Name"" AS FloorName,
                                    P.""Id"" AS PatientId,
                                    P.""FirstName"",
                                    P.""MiddleName"",
                                    P.""LastName"",
                                    P.""MRN"",
                                    P.""Gender"",
                                    P.""DateOfBirth"",
                                    R.""Name"" AS RoomName,
                                    R.""Id"" AS RoomId,
                                    PVH.""UserId"" AS PatientUserId,
                                    PVH.""IsActive"" AS Status,
                                    COUNT(PVH.""Id"") OVER(PARTITION BY F.""Id"") AS PatientCount
                                FROM LatestVisit AS PVHD
                                JOIN public.""Buildings"" AS B ON PVHD.""BuildingId"" = B.""Id""
                                JOIN public.""PatientVisitHistory"" AS PVH ON PVH.""Id"" = PVHD.""PatientVisitHistoryId""
                                JOIN public.""Patients"" AS P ON PVH.""PatientId"" = P.""Id""
                                JOIN public.""Floors"" AS F ON PVHD.""FloorId"" = F.""Id""
                                JOIN public.""Rooms"" AS R ON PVHD.""RoomId"" = R.""Id""
                                WHERE PVHD.""BuildingId"" = {buildingId} AND PVH.""PatientCensusStatus"" = 1
                                ORDER BY R.""Id"";
                            ";


                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var floorDictionary = new Dictionary<int, FloorList>();
                            while (await reader.ReadAsync())
                            {
                                if (buildingPatientList.BuildingId == 0)
                                {
                                    buildingPatientList.BuildingId = reader.GetInt32(reader.GetOrdinal("BuildingId"));
                                    buildingPatientList.BuildingName = reader.GetString(reader.GetOrdinal("BuildingName"));
                                    buildingPatientList.BuildingLocation = reader.GetString(reader.GetOrdinal("BuildingLocation"));
                                }
                                int floorId = reader.GetInt32(reader.GetOrdinal("FloorId"));
                                if (!floorDictionary.ContainsKey(floorId))
                                {
                                    var floor = new FloorList
                                    {
                                        FloorName = reader.GetString(reader.GetOrdinal("FloorName")),
                                        FloorId = reader.GetInt32(reader.GetOrdinal("FloorId"))
                                    };
                                    floorDictionary[floorId] = floor;
                                    buildingPatientList.Floors.Add(floor);
                                }
                                var firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName"));
                                var middleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName"));
                                var lastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName"));
                                var patient = new PatientList
                                {
                                    PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                                    FullName = $"{firstName} {middleName} {lastName}".Trim(),
                                    MRN = reader.GetString(reader.GetOrdinal("MRN")),
                                    Gender = reader.GetInt32(reader.GetOrdinal("Gender")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("PatientUserId")),
                                    IsActive = reader.GetInt32(reader.GetOrdinal("Status")),
                                    DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                    RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                                    RoomId = reader.GetInt32(reader.GetOrdinal("RoomId"))
                                };

                                floorDictionary[floorId].Patients.Add(patient);
                            }

                            buildingPatientList.PatientCount = floorDictionary.Values.Sum(f => f.Patients.Count);
                            buildingPatientList.FloorCount = floorDictionary.Count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return buildingPatientList;
        }

        public async Task<bool> DeleteBuilding(int buildingId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    string sqlQuery = $"DELETE FROM \"Buildings\" WHERE \"Id\"={buildingId}";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);

                    await connection.OpenAsync();
                    int affectedRows = await command.ExecuteNonQueryAsync();

                    return affectedRows > 0;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Building> GetBuildingById(int Id)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    string sqlQuery = $@"
                Select 
                    b.""Id"" as BuildingId, 
                    b.""BuildingName"", 
                    b.""BuildingLocation"", 
                    b.""NoOfFloors"", b.""FloorRange1"", b.""FloorRange2"", 
                    f.""Id"" as FloorId, 
                    f.""Name"" as FloorName, 
                    f.""RoomRange"" as RoomRange, 
                    r.""Id"" as RoomId, r.""FloorId"" as FloorId, 
                    r.""Name"" as RoomName, 
                    r.""Description"" as RoomDescription
                FROM 
                    ""Buildings"" b 
                LEFT JOIN 
                    ""Floors"" f ON f.""BuildingId"" = b.""Id"" AND f.""IsActive"" = 1
                LEFT JOIN 
                    ""Rooms"" r ON r.""FloorId"" = f.""Id"" AND r.""IsActive"" = 1
                WHERE 
                    b.""Id""=@id AND b.""IsActive"" = 1;
            ";

                    using (var command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        // Add the parameter to the query to prevent SQL injection
                        command.Parameters.AddWithValue("id", Id);

                        // Open the connection asynchronously
                        await connection.OpenAsync();

                        // Execute the query and process the results
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            Building building = new();
                            building.Floors = new List<Floor>();
                            while (await reader.ReadAsync())
                            {
                                building.Id = reader.GetInt32(reader.GetOrdinal("BuildingId"));
                                building.Name = reader.GetString(reader.GetOrdinal("BuildingName"));
                                building.FloorRange1 = reader.GetInt32(reader.GetOrdinal("FloorRange1"));
                                building.FloorRange2 = reader.GetInt32(reader.GetOrdinal("FloorRange2"));
                                building.BuildingLocation = reader.GetString(reader.GetOrdinal("BuildingLocation"));
                                building.NoOfFloors = reader.GetInt32(reader.GetOrdinal("NoOfFloors"));
                                // Add floor details if available
                                var floorId = reader.IsDBNull(reader.GetOrdinal("FloorId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("FloorId"));
                                if (floorId.HasValue)
                                {
                                    var existingFloor = building.Floors.FirstOrDefault(f => f.Id == floorId.Value);
                                    Floor floor;
                                    if (existingFloor == null)
                                    {
                                        floor = new Floor()
                                        {
                                            Id = floorId.Value,
                                            Name = reader.GetString(reader.GetOrdinal("FloorName")),
                                            RoomRange = reader.IsDBNull(reader.GetOrdinal("RoomRange")) ? "0" : reader.GetString(reader.GetOrdinal("RoomRange")),
                                            Rooms = new List<Room>()
                                        };
                                        building.Floors.Add(floor);
                                    }
                                    else
                                    {
                                        floor = existingFloor;
                                    }

                                    // Add room details if available
                                    var roomId = reader.IsDBNull(reader.GetOrdinal("RoomId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RoomId"));
                                    if (roomId.HasValue)
                                    {
                                        var existingRoom = floor.Rooms.FirstOrDefault(r => r.Id == roomId.Value);
                                        Room room;
                                        if (existingRoom == null)
                                        {
                                            room = new Room()
                                            {
                                                Id = roomId.Value,
                                                Name = reader.GetString(reader.GetOrdinal("RoomName")),
                                                FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                                            };
                                            floor.Rooms.Add(room);
                                        }
                                        else
                                        {
                                            room = existingRoom;
                                        }
                                    }
                                }
                            }

                            return building;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (optional, depending on your logging mechanism)
                Console.Error.WriteLine($"Error fetching building details: {ex.Message}");

                // Handle the exception (rethrow or return a specific response)
                throw new Exception("An error occurred while fetching the building data.", ex);
            }

        }

        public async Task<ResponseMessage> UpdateBuilding(Building model)
        {
            try
            {
                var building = await _MedChgContext.Buildings.FindAsync(model.Id);

                if (building == null)
                {
                    return new ResponseMessage
                    {
                        Success = false,
                        Message = "Building not found",
                        ErrorCode = "BUILDING_NOT_FOUND",
                        Errors = new List<string> { "Building not found" }
                    };
                }
                building.Name = model.Name;
                building.BuildingLocation = model.BuildingLocation;
                building.NoOfFloors = model.NoOfFloors;
                building.FloorRange1 = model.FloorRange1;
                building.FloorRange2 = model.FloorRange2;
                building.IsActive = model.IsActive;
                building.UpdatedAt = TimeHelper.GetTexasTime();
                building.UpdatedById = model.UpdatedById;
                building.Device = model.Device;

                await _MedChgContext.SaveChangesAsync();
                return new ResponseMessage
                {
                    Success = true,
                    Message = "Building Updated successfully",
                    Data = building
                };
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "SERVER_ERROR",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}

