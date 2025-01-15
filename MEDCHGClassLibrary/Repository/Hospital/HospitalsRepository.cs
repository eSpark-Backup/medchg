using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Services;
using MEDCHGClassLibrary.Utilities.Helpers;
using MEDCHGClassLibrary.ViewModels.Building;
using MEDCHGClassLibrary.ViewModels.Hospital;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Repository.Hospital
{
    public class HospitalsRepository : IHospitalRepository
    {
        private string _npgsqlDataSource = null!;

        public string NpgsqlDataSource
        {
            get { return _npgsqlDataSource; }
            set { _npgsqlDataSource = value; }
        }

        private IConfiguration _configureOptions;

        private readonly MedChgContext _MedChgContext;
        public HospitalsRepository(IConfiguration configureOptions, MedChgContext medChgContext)
        {
            _configureOptions = configureOptions;
            _MedChgContext = medChgContext;
            _npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection")!;
        }
        public async Task<ResponseMessage> AddHospital(Model.Hospital model)
        {
            var response = new ResponseMessage
            {
                Success = true,
                Message = "Hospital and associated buildings and floors added successfully.",
                ErrorCode = string.Empty,
                Errors = new List<string>()
            };
            try
            {
                model.CreatedAt = TimeHelper.GetTexasTime();
                model.Date = TimeHelper.GetTexasTime();
                model.Device = "Web"; // This can be set dynamically or based on request headers
                // Step 2: Add the hospital entity to the context and save it
                _MedChgContext.Hospitals.Add(model);
                await _MedChgContext.SaveChangesAsync();
                response.Data = model;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while processing your request.";
                response.ErrorCode = "ERR-001"; // You can assign a more specific error code
                response.Errors.Add(ex.Message); // Add the exception message to the errors list
            }
            return response;
        }


        public async Task<HospitalListVm> GetHospitalWithProvider(int providerId, int userId)
        {
            var hospitalListVm = new HospitalListVm
            {
                ProviderId = providerId
            };
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    var sqlQuery = $@"
                            SELECT 
                                h.""Id"",
                                h.""HospitalName"",
                                p.""DisplayName"" AS ""ProviderDisplayName"",
                                COUNT(DISTINCT pvh.""Id"") FILTER (WHERE pvh.""PatientCensusStatus"" = 1) AS ""PatCount"",
                                COALESCE(uh.""IsFavourite"", FALSE) AS ""Status""
                                FROM 
                                ""Hospitals"" h
                                JOIN 
                                ""Providers"" p ON h.""ProviderId"" = p.""Id""
                                 LEFT JOIN 
                                ""PatientVisitHistoryDetailLists"" pvhd ON h.""Id"" = pvhd.""HospitalId""
                                 LEFT JOIN 
                                ""PatientVisitHistory"" pvh ON pvhd.""PatientVisitHistoryId"" = pvh.""Id""
                                LEFT JOIN 
                                ""UsersHospitals"" uh ON h.""Id"" = uh.""HospitalId"" AND uh.""UserId"" = {userId}
                                WHERE
                                h.""ProviderId"" = {providerId}
                                GROUP BY
                                h.""Id"", h.""HospitalName"", p.""DisplayName"", uh.""IsFavourite""
                                ORDER BY 
                                ""Status"" DESC;";

                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (hospitalListVm.ProviderName == null)
                                {
                                    hospitalListVm.ProviderName = reader.GetString(reader.GetOrdinal("ProviderDisplayName"));
                                }
                                var hospitalItem = new HospitalListItemVm
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("HospitalName")),
                                    PatientVisitCount = reader.GetInt32(reader.GetOrdinal("PatCount")),
                                    Status = !reader.IsDBNull(reader.GetOrdinal("Status")) ? reader.GetBoolean(reader.GetOrdinal("Status")) : false
                                };
                                hospitalListVm.Items.Add(hospitalItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return hospitalListVm;
        }

        public async Task<List<Model.Hospital>> GetAllHospitals()
        {
            List<Model.Hospital> Hospitals = new List<Model.Hospital>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"Hospitals\" where \"IsActive\"=1";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Model.Hospital Hospital = new Model.Hospital
                        {
                            // Map database columns to ApplicationUser properties
                            HospitalName = reader.GetString(reader.GetOrdinal("HospitalName")),
                            ProviderId = reader.GetInt32(reader.GetOrdinal("ProviderId")),
                            Id = reader.GetInt32(reader.GetOrdinal("Id"))
                            // Add other properties as needed
                        };
                        Hospitals.Add(Hospital);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return Hospitals;
        }

        public Task<Model.Hospital> GetAllHospitalsWithThereForeignKeys(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<CountVM> GetHospitalById(int id)
        {
            try
            {
                // Use 'using' to ensure the connection is disposed of properly.
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    // Open the connection asynchronously.
                    await connection.OpenAsync();

                    // Define the SQL query with parameterized SQL to avoid SQL injection.
                    string sqlQuery = @"
                                        SELECT 
                                            h.""Id"" AS ""HospitalId"",
                                            h.""HospitalName"",
                                            b.""Id"" AS ""BuildingId"",
                                            b.""BuildingName"", 
                                            b.""BuildingLocation"",
                                            COUNT(DISTINCT f.""Id"") AS ""FloorCount"",  -- Counting distinct floors for each building
                                            f.""Id"" AS ""FloorId"",
                                            COUNT(r.""Id"") AS ""RoomCount""  -- Counting rooms for each floor
                                        FROM 
                                            ""Hospitals"" h
                                        JOIN 
                                            ""Buildings"" b ON h.""Id"" = b.""HospitalId""
                                        JOIN 
                                            ""Floors"" f ON b.""Id"" = f.""BuildingId""
                                        LEFT JOIN 
                                            ""Rooms"" r ON f.""Id"" = r.""FloorId""AND r.""IsActive"" = 1
                                        WHERE 
                                            h.""Id"" = @id
                                            AND h.""IsActive"" = 1  -- Only include active hospitals
                                            AND b.""IsActive"" = 1  -- Only include active buildings
                                            AND f.""IsActive"" = 1  -- Only include active floors
                                        GROUP BY 
                                            h.""Id"", f.""Id"", h.""HospitalName"", b.""BuildingLocation"", b.""BuildingName"", b.""Id""
                                        ORDER BY 
                                            h.""Id"", b.""Id"", f.""Id"";";

                    // Create the command and add the parameter for 'id'.
                    using (var command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        // Execute the query asynchronously and get the reader.
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // List to hold building details
                            var buildings = new List<BuildingDetailVM>();

                            // Variables to store hospital name
                            string hospitalName = string.Empty;

                            // Loop through the data reader to read the results
                            while (await reader.ReadAsync())
                            {
                                // Initialize the hospital name on the first row
                                if (string.IsNullOrEmpty(hospitalName))
                                {
                                    hospitalName = reader.GetString(reader.GetOrdinal("HospitalName"));
                                }

                                // Get the building details
                                int buildingId = reader.GetInt32(reader.GetOrdinal("BuildingId"));
                                string buildingName = reader.GetString(reader.GetOrdinal("BuildingName"));  // Added BuildingName
                                string buildingLocation = reader.GetString(reader.GetOrdinal("BuildingLocation"));
                                int floorCount = reader.GetInt32(reader.GetOrdinal("FloorCount"));
                                int floorId = reader.GetInt32(reader.GetOrdinal("FloorId"));
                                int roomCount = reader.GetInt32(reader.GetOrdinal("RoomCount"));

                                // Check if the building is already added
                                var building = buildings.FirstOrDefault(b => b.BuildingId == buildingId);

                                if (building == null)
                                {
                                    // If the building is not in the list, add it
                                    building = new BuildingDetailVM
                                    {
                                        BuildingId = buildingId,
                                        BuildingName = buildingName,  // Set BuildingName
                                        BuildingLocation = buildingLocation,
                                        FloorCount = floorCount,
                                        Floors = new List<FloorDetailVM>()
                                    };

                                    buildings.Add(building);
                                }

                                // Add the floor details to the building
                                building.Floors.Add(new FloorDetailVM
                                {
                                    FloorId = floorId,
                                    RoomCount = roomCount
                                });
                            }

                            // Create the CountVM to return
                            var countVM = new CountVM
                            {
                                HospitalId = id,
                                HospitalName = hospitalName,
                                Buildings = buildings
                            };

                            return countVM;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (optional, depending on your logging mechanism)
                Console.Error.WriteLine($"Error fetching hospital details: {ex.Message}");

                // Handle the exception (rethrow or return a specific response)
                throw new Exception("An error occurred while fetching the hospital data.", ex);
            }
        }
        public async Task<HospitalBuildingListVm> GetHospitalBuildings(int hospitalId)
        {
            try
            {
                HospitalBuildingListVm hospitalBuildingVm = new HospitalBuildingListVm
                {
                    HospitalId = hospitalId,
                    HospitalName = null,
                    HospitalBuildings = new List<HospitalBuildingList>()
                };

                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    string sqlQuery = $@"
                                         WITH RankedVisits AS (
                                                        SELECT 
                                                            pvd.""PatientVisitHistoryId"", 
                                                            pvd.""HospitalId"", 
                                                            pvd.""BuildingId"", 
                                                            pvd.""CreatedAt"",
                                                            ROW_NUMBER() OVER (PARTITION BY pvd.""PatientVisitHistoryId"" ORDER BY pvd.""CreatedAt"" DESC) AS rn
                                                        FROM ""PatientVisitHistoryDetailLists"" pvd
                                                        JOIN ""PatientVisitHistory"" pv ON pvd.""PatientVisitHistoryId"" = pv.""Id""  -- Join with PatientVisitHistory table
                                                        WHERE pv.""PatientCensusStatus"" = 1 
                                                    ),
                                                    FloorCounts AS (
                                                        SELECT 
                                                            ""BuildingId"", 
                                                            COUNT(*) AS floor_count
                                                        FROM ""Floors""
                                                        WHERE ""IsActive"" = 1  
                                                        GROUP BY ""BuildingId""
                                                    )
                                                    SELECT 
                                                        rv.""HospitalId"", 
                                                        rv.""BuildingId"", 
                                                        h.""HospitalName"", 
                                                        b.""BuildingName"", 
                                                        b.""BuildingLocation"",
                                                        COUNT(DISTINCT rv.""PatientVisitHistoryId"") AS patient_count,
                                                        fc.floor_count
                                                    FROM RankedVisits rv
                                                    JOIN ""Buildings"" b ON rv.""BuildingId"" = b.""Id""  
                                                    JOIN FloorCounts fc ON rv.""BuildingId"" = fc.""BuildingId""  
                                                    JOIN ""Hospitals"" h ON rv.""HospitalId"" = h.""Id"" 
                                                    WHERE rv.rn = 1  
                                                    AND rv.""HospitalId"" = {hospitalId}  
                                                    GROUP BY rv.""HospitalId"", h.""HospitalName"", rv.""BuildingId"", b.""BuildingName"", b.""BuildingLocation"", fc.floor_count;";
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (hospitalBuildingVm.HospitalName == null)
                                {
                                    hospitalBuildingVm.HospitalName = reader.GetString(reader.GetOrdinal("HospitalName"));
                                }

                                HospitalBuildingList hospitalBuilding = new HospitalBuildingList
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("BuildingId")),
                                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                                    BuildingLocation = reader.GetString(reader.GetOrdinal("BuildingLocation")),
                                    FloorCount = reader.GetInt32(reader.GetOrdinal("floor_count")),
                                    PatientCount = reader.GetInt32(reader.GetOrdinal("patient_count"))
                                };

                                hospitalBuildingVm.HospitalBuildings.Add(hospitalBuilding);
                            }
                        }
                    }
                }
                return hospitalBuildingVm;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> DeleteHospital(int hospitalId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    string sqlQuery = $"DELETE FROM \"Hospitals\" WHERE \"Id\"={hospitalId}";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@Id", hospitalId);

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

        public async Task<ResponseMessage> CheckHospitalNameUnique(Model.Hospital model)
        {
            var response = new ResponseMessage
            {
                Success = true,
                Message = "Hospital name is unique for the given provider.",
                ErrorCode = string.Empty,
                Errors = new List<string>()
            };

            try
            {
                // Query the database to check if there is any hospital with the same name and provider ID
                var existingHospital = await _MedChgContext.Hospitals
                    .FirstOrDefaultAsync(h => h.HospitalName == model.HospitalName && h.ProviderId == model.ProviderId);

                // If a hospital with the same name and provider ID exists, return a failure response
                if (existingHospital != null)
                {
                    response.Success = false;
                    response.Message = "A hospital with the same name already exists for this provider.";
                    response.Errors.Add($"Hospital with name '{model.HospitalName}' already exists for provider {model.ProviderId}.");
                    return response;
                }
            }
            catch (Exception ex)
            {
                // Log the error (use a logging framework like Serilog, NLog, etc.)
                Console.Error.WriteLine(ex.Message);

                response.Success = false;
                response.Message = "An error occurred while checking the hospital name.";
                response.ErrorCode = "ERR-002"; // Assign a specific error code
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<Model.Hospital> GetHospitalBuildingsRooms(int hospitalId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    // Open the database connection
                    await connection.OpenAsync();

                    // SQL query to get the hospital, buildings, floors, and rooms
                    string sql = @"
                                    SELECT
                                        ""b"".""Id"" AS ""BuildingId"",
                                        ""b"".""BuildingName"",
                                        ""b"".""BuildingLocation"",
                                        ""b"".""NoOfFloors"",
                                        ""b"".""FloorRange1"",
                                        ""b"".""FloorRange2"",
                                        ""b"".""HospitalId"",
                                        ""b"".""IsActive"" AS ""BuildingIsActive"",
                                        ""b"".""CreatedAt"" AS ""BuildingCreatedAt"",
                                        ""b"".""Date"" AS ""BuildingDate"",
                                        ""b"".""CreatedById"" AS ""BuildingCreatedById"",
                                        ""b"".""UpdatedAt"" AS ""BuildingUpdatedAt"",
                                        ""b"".""UpdatedById"" AS ""BuildingUpdatedById"",
                                        ""b"".""Device"" AS ""BuildingDevice"",
                                        ""f"".""Id"" AS ""FloorId"",
                                        ""f"".""Name"" AS ""FloorName"",
                                        ""f"".""RoomCode"",
                                        ""f"".""RoomRange"",
                                        ""f"".""description"" AS ""FloorDescription"",
                                        ""f"".""IsActive"" AS ""FloorIsActive"",
                                        ""f"".""CreatedAt"" AS ""FloorCreatedAt"",
                                        ""f"".""Date"" AS ""FloorDate"",
                                        ""f"".""CreatedById"" AS ""FloorCreatedById"",
                                        ""f"".""UpdatedAt"" AS ""FloorUpdatedAt"",
                                        ""f"".""UpdatedById"" AS ""FloorUpdatedById"",
                                        ""f"".""Device"" AS ""FloorDevice"",
                                        ""r"".""Id"" AS ""RoomId"",
                                        ""r"".""Name"" AS ""RoomName"",
                                        ""r"".""Description"" AS ""RoomDescription"",
                                        ""r"".""IsActive"" AS ""RoomIsActive"",
                                        ""r"".""CreatedAt"" AS ""RoomCreatedAt"",
                                        ""r"".""Date"" AS ""RoomDate"",
                                        ""r"".""CreatedById"" AS ""RoomCreatedById"",
                                        ""r"".""UpdatedAt"" AS ""RoomUpdatedAt"",
                                        ""r"".""UpdatedById"" AS ""RoomUpdatedById"",
                                        ""r"".""Device"" AS ""RoomDevice""
                                    FROM
                                        ""Buildings"" ""b""
                                    JOIN ""Floors"" ""f"" ON ""b"".""Id"" = ""f"".""BuildingId""
                                    JOIN ""Rooms"" ""r"" ON ""f"".""Id"" = ""r"".""FloorId""
                                    WHERE
                                        ""b"".""HospitalId"" = @HospitalId;";

                    var buildings = new List<Building>();

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@HospitalId", hospitalId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            Building currentBuilding = null;
                            Floor currentFloor = null;

                            while (await reader.ReadAsync())
                            {
                                // Read Building Information
                                int buildingId = reader.GetInt32(reader.GetOrdinal("BuildingId"));
                                string buildingName = reader.IsDBNull(reader.GetOrdinal("BuildingName")) ? null : reader.GetString(reader.GetOrdinal("BuildingName"));
                                string buildingLocation = reader.IsDBNull(reader.GetOrdinal("BuildingLocation")) ? null : reader.GetString(reader.GetOrdinal("BuildingLocation"));
                                int? noOfFloors = reader.IsDBNull(reader.GetOrdinal("NoOfFloors")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("NoOfFloors"));
                                int floorRange1 = reader.GetInt32(reader.GetOrdinal("FloorRange1"));
                                int floorRange2 = reader.GetInt32(reader.GetOrdinal("FloorRange2"));
								int isBuildingActive = reader.GetInt32(reader.GetOrdinal("BuildingIsActive"));
                                DateTime createdAt = reader.GetDateTime(reader.GetOrdinal("BuildingCreatedAt"));
                                DateTime buildingDate = reader.GetDateTime(reader.GetOrdinal("BuildingDate"));
                                int? createdById = reader.IsDBNull(reader.GetOrdinal("BuildingCreatedById")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BuildingCreatedById"));
                                int? updatedById = reader.IsDBNull(reader.GetOrdinal("BuildingUpdatedById")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BuildingUpdatedById"));
                                string device = reader.IsDBNull(reader.GetOrdinal("BuildingDevice")) ? null : reader.GetString(reader.GetOrdinal("BuildingDevice"));

                                // Check if the current building is a new one
                                if (currentBuilding == null || currentBuilding.Id != buildingId)
                                {
                                    if (currentBuilding != null)
                                    {
                                        buildings.Add(currentBuilding); // Add previous building to the list
                                    }

                                    // Create a new Building object
                                    currentBuilding = new Building
                                    {
                                        Id = buildingId,
                                        Name = buildingName,
                                        BuildingLocation = buildingLocation,
                                        NoOfFloors = noOfFloors,
                                        FloorRange1 = floorRange1,
                                        FloorRange2 = floorRange2,
                                        IsActive = isBuildingActive,
                                        CreatedAt = createdAt,
                                        Date = buildingDate,
                                        CreatedById = createdById,
                                        UpdatedById = updatedById,
                                        Device = device,
                                        Floors = new List<Floor>()
                                    };
                                }

                                // Read Floor Information
                                int floorId = reader.GetInt32(reader.GetOrdinal("FloorId"));
                                string floorName = reader.IsDBNull(reader.GetOrdinal("FloorName")) ? null : reader.GetString(reader.GetOrdinal("FloorName"));
                                string floorRoomCode = reader.IsDBNull(reader.GetOrdinal("RoomCode")) ? null : reader.GetString(reader.GetOrdinal("RoomCode"));
                                string roomRange = reader.IsDBNull(reader.GetOrdinal("RoomRange")) ? null : reader.GetString(reader.GetOrdinal("RoomRange"));
                                string floorDescription = reader.IsDBNull(reader.GetOrdinal("FloorDescription")) ? null : reader.GetString(reader.GetOrdinal("FloorDescription"));
                                int isFloorActive = reader.GetInt32(reader.GetOrdinal("FloorIsActive"));
                                DateTime floorCreatedAt = reader.GetDateTime(reader.GetOrdinal("FloorCreatedAt"));
                                DateTime floorDate = reader.GetDateTime(reader.GetOrdinal("FloorDate"));
                                int? floorCreatedById = reader.IsDBNull(reader.GetOrdinal("FloorCreatedById")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("FloorCreatedById"));
                                int? floorUpdatedById = reader.IsDBNull(reader.GetOrdinal("FloorUpdatedById")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("FloorUpdatedById"));
                                string floorDevice = reader.IsDBNull(reader.GetOrdinal("FloorDevice")) ? null : reader.GetString(reader.GetOrdinal("FloorDevice"));

                                // Check if the current floor is a new one
                                if (currentFloor == null || currentFloor.Id != floorId)
                                {
                                    if (currentFloor != null)
                                    {
                                        currentBuilding.Floors.Add(currentFloor); // Add previous floor to the list
                                    }

                                    // Create a new Floor object
                                    currentFloor = new Floor
                                    {
                                        Id = floorId,
                                        Name = floorName,
                                        RoomCode = floorRoomCode,
                                        RoomRange = roomRange,
                                        description = floorDescription,
                                        IsActive = isFloorActive,
                                        CreatedAt = floorCreatedAt,
                                        Date = floorDate,
                                        CreatedById = floorCreatedById,
                                        UpdatedById = floorUpdatedById,
                                        Device = floorDevice,
                                        Rooms = new List<Room>()
                                    };
                                }

                                // Read Room Information
                                int roomId = reader.GetInt32(reader.GetOrdinal("RoomId"));
                                string roomName = reader.IsDBNull(reader.GetOrdinal("RoomName")) ? null : reader.GetString(reader.GetOrdinal("RoomName"));
                                string roomDescription = reader.IsDBNull(reader.GetOrdinal("RoomDescription")) ? null : reader.GetString(reader.GetOrdinal("RoomDescription"));
                                string roomCode = reader.IsDBNull(reader.GetOrdinal("RoomCode")) ? null : reader.GetString(reader.GetOrdinal("RoomCode"));
                                int isRoomActive = reader.GetInt32(reader.GetOrdinal("RoomIsActive"));
                                DateTime roomCreatedAt = reader.GetDateTime(reader.GetOrdinal("RoomCreatedAt"));
                                DateTime roomDate = reader.GetDateTime(reader.GetOrdinal("RoomDate"));
                                int? roomCreatedById = reader.IsDBNull(reader.GetOrdinal("RoomCreatedById")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RoomCreatedById"));
                                int? roomUpdatedById = reader.IsDBNull(reader.GetOrdinal("RoomUpdatedById")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RoomUpdatedById"));
                                string roomDevice = reader.IsDBNull(reader.GetOrdinal("RoomDevice")) ? null : reader.GetString(reader.GetOrdinal("RoomDevice"));

                                // Create a new Room object
                                Room room = new Room
                                {
                                    Id = roomId,
                                    RoomCode = roomCode,
                                    Description = roomDescription,
                                    IsActive = isRoomActive,
                                    CreatedAt = roomCreatedAt,
                                    Date = roomDate,
                                    CreatedById = roomCreatedById,
                                    UpdatedById = roomUpdatedById,
                                    Device = roomDevice
                                };
                                room.Name = roomName;

                                // Add the room to the current floor
                                currentFloor?.Rooms.Add(room);
                            }

                            // Add the last building to the list (if any)
                            if (currentBuilding != null)
                            {
                                buildings.Add(currentBuilding);
                            }
                        }
                    }

                    // Map to Hospital model (assuming you have a Hospital model)
                    return new Model.Hospital
                    {
                        Id = hospitalId,
                        Buildings = buildings
                    };
                }
            }
            catch (Exception ex)
            {
                // Log exception here (e.g., using a logging framework)
                throw new ApplicationException("An error occurred while retrieving data.", ex);
            }
        }


    }
}
