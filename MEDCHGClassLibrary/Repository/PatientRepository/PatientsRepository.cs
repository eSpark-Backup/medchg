using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Utilities.Helpers;
using MEDCHGClassLibrary.ViewModels.Patient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Net.Http.Json;

namespace MEDCHGClassLibrary.Repository.PatientRepository
{

	public class PatientsRepository : IPatientRepository
	{
		private string _npgsqlDataSource;

		public string NpgsqlDataSource
		{
			get { return _npgsqlDataSource; }
			set { _npgsqlDataSource = value; }
		}

		private IConfiguration _configureOptions;

		private readonly MedChgContext _MedChgContext;
		public PatientsRepository(IConfiguration configureOptions, MedChgContext medChgContext)
		{
			_configureOptions = configureOptions;
			_MedChgContext = medChgContext;
			_npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection");
		}

		public async Task<PatientInfoVm> GetPatientInfo(int patientId)
		{
			try
			{
				using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
				{
					var sqlQuery = $@"
                    SELECT 
                    P.*,
                    PVH.*,
                    R.""Name"" AS ""RoomName"", 
                    F.""Name"" AS ""FloorName"", 
                    B.""BuildingName"" AS ""BuildingName"", 
                    H.""HospitalName"" AS ""HospitalName"", 
                    Pro.""Name"" AS ""ProviderName"",
                    U.""FullName"" AS ""Practitioner"",  
                    U.""Id"" AS ""PractitionerId"",
                    PVH.""UserId"" AS ""Users"", 
                    PVH.""Id"" AS ""VisitHistoryId"",
                    PVH.""IsActive"",
                    PVH.""PatientAddmissionDate"" AS ""HistoryPatientAddmissionDateTime"", 
                    PVH.""CreatedAt"" AS ""VisitingDateTime"",
                    PVHDL.""Id"" AS ""VisitHistoryDetailId"",
                    PVHDL.""CodeName"" AS ""Code"",
                    PVHDL.""CodeDescription"" AS ""Description"",
                    PVHDL.""CodeType"" AS ""Type"",
                    PVHDL.""PractitionerId"" AS ""UserId"",
                    PVHDL.""PractitionerName"" AS ""UserName"",
                    PVHDL.""CreatedAt"" AS ""CreatedDateTime"",
                    PVH.""PatientCensusStatus"" AS ""State"",
                    PVHDL.""RoomId"" AS ""PVHD_RoomId"",
                    PVHDL.""FloorId"" AS ""PVHD_FloorId"",
                    PVHDL.""BuildingId"" AS ""PVHD_BuildingId"",
                    PVHDL.""HospitalId"" AS ""PVHD_HospitalId""
                FROM 
                    public.""Patients"" AS P
                JOIN 
                    public.""PatientVisitHistory"" AS PVH ON P.""Id"" = PVH.""PatientId"" AND PVH.""Id"" = (
                        SELECT PVH2.""Id""
                        FROM public.""PatientVisitHistory"" AS PVH2
                        WHERE PVH2.""PatientId"" = PVH.""PatientId""
                        ORDER BY PVH2.""CreatedAt"" DESC
                        LIMIT 1
                    )
                JOIN 
                    public.""Users"" AS U ON PVH.""UserId"" = U.""Id""
                LEFT JOIN 
                    public.""PatientVisitHistoryDetailLists"" AS PVHDL ON PVH.""Id"" = PVHDL.""PatientVisitHistoryId""
                LEFT JOIN 
                    public.""Rooms"" AS R ON PVHDL.""RoomId"" = R.""Id""
                LEFT JOIN 
                    public.""Floors"" AS F ON PVHDL.""FloorId"" = F.""Id""
                LEFT JOIN 
                    public.""Buildings"" AS B ON PVHDL.""BuildingId"" = B.""Id""
                LEFT JOIN 
                    public.""Hospitals"" AS H ON PVHDL.""HospitalId"" = H.""Id""
                LEFT JOIN 
                    public.""Providers"" AS Pro ON H.""ProviderId"" = Pro.""Id""
                WHERE 
                    P.""Id"" = {patientId}
                    ORDER BY ""CreatedDateTime"" ASC;";

					await connection.OpenAsync();
					PatientInfoVm patientInfo = null!;
					// Temporary variables to store the last row values for Hospital, Building, Floor, and Room
					int? lastHospitalId = null, lastBuildingId = null, lastFloorId = null, lastRoomId = null;
					string lastRoomName = "", lastFloorName = "", lastBuildingName = "", lastHospitalName = "";

					using (var command = new NpgsqlCommand(sqlQuery, connection))
					{
						using (var reader = await command.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								// Always fetch the patient info from the first row (if it's not already set)
								if (patientInfo == null)
								{
									var firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName"));
									var middleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName"));
									var lastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName"));
									var dateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? (DateOnly?)null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth")));

									patientInfo = new PatientInfoVm
									{
										Id = reader.GetInt32(reader.GetOrdinal("Id")),
										UserId = reader.GetInt32(reader.GetOrdinal("PractitionerId")),
										Age = dateOfBirth.HasValue ? CalCulateAge.AgeCalculator(dateOfBirth.Value) : (int?)null,
										DateOfBirth = dateOfBirth,
										FirstName = firstName,
										MiddleName = middleName,
										LastName = lastName,
										CreatedAt = reader.IsDBNull(reader.GetOrdinal("VisitingDateTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("VisitingDateTime")),
										FullName = $"{firstName} {middleName} {lastName}".Trim(),
										MRN = reader.IsDBNull(reader.GetOrdinal("MRN")) ? "" : reader.GetString(reader.GetOrdinal("MRN")),
										Salutation = reader.IsDBNull(reader.GetOrdinal("Salutation")) ? "" : reader.GetString(reader.GetOrdinal("Salutation")),
										Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? 0 : reader.GetInt32(reader.GetOrdinal("Gender")),
										PatientVisitsId = 0,
										PatientVisitHistoryId = reader.GetInt32(reader.GetOrdinal("VisitHistoryId")),
										Practitioner = reader.IsDBNull(reader.GetOrdinal("Practitioner")) ? "" : reader.GetString(reader.GetOrdinal("Practitioner")),
										AdmissionDate = reader.IsDBNull(reader.GetOrdinal("PatientAddmissionDateTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientAddmissionDateTime")),
										CensusStatus = reader.GetInt32(reader.GetOrdinal("State"))
									};
								}

								// Set the last row values for Hospital, Building, Floor, and Room
								lastRoomName = reader.IsDBNull(reader.GetOrdinal("RoomName")) ? "" : reader.GetString(reader.GetOrdinal("RoomName"));
								lastFloorName = reader.IsDBNull(reader.GetOrdinal("FloorName")) ? "" : reader.GetString(reader.GetOrdinal("FloorName"));
								lastBuildingName = reader.IsDBNull(reader.GetOrdinal("BuildingName")) ? "" : reader.GetString(reader.GetOrdinal("BuildingName"));
								lastHospitalName = reader.IsDBNull(reader.GetOrdinal("HospitalName")) ? "" : reader.GetString(reader.GetOrdinal("HospitalName"));
								lastHospitalId = reader.IsDBNull(reader.GetOrdinal("PVHD_HospitalId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PVHD_HospitalId"));
								lastBuildingId = reader.IsDBNull(reader.GetOrdinal("PVHD_BuildingId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PVHD_BuildingId"));
								lastFloorId = reader.IsDBNull(reader.GetOrdinal("PVHD_FloorId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PVHD_FloorId"));
								lastRoomId = reader.IsDBNull(reader.GetOrdinal("PVHD_RoomId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PVHD_RoomId"));

								var codesName = reader.IsDBNull(reader.GetOrdinal("Code")) ? "" : reader.GetString(reader.GetOrdinal("Code"));
								var censusState = reader.IsDBNull(reader.GetOrdinal("State")) ? 0 : reader.GetInt32(reader.GetOrdinal("State"));

								if (codesName != null && (censusState == 0 || censusState == 1))
								{
									var visit = new PatientVisits
									{
										PatientId = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
										PatientVisitHistoryId = reader.IsDBNull(reader.GetOrdinal("VisitHistoryId")) ? 0 : reader.GetInt32(reader.GetOrdinal("VisitHistoryId")),
										PatientVisitHistoryDetailId = reader.IsDBNull(reader.GetOrdinal("VisitHistoryDetailId")) ? 0 : reader.GetInt32(reader.GetOrdinal("VisitHistoryDetailId")),
										CodeName = codesName,
										CodeDescription = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
										Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? "" : reader.GetString(reader.GetOrdinal("Type")),
										Username = reader.IsDBNull(reader.GetOrdinal("UserName")) ? "" : reader.GetString(reader.GetOrdinal("UserName")),
										CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedDateTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedDateTime"))
									};

									patientInfo.Visits.Add(visit);
								}
							}
						}
					}

					// Set the Hospital, Building, Floor, and Room info from the last row
					if (patientInfo != null)
					{
						patientInfo.HospitalId = lastHospitalId ?? 0;
						patientInfo.HospitalName = lastHospitalName;
						patientInfo.BuildingId = lastBuildingId ?? 0;
						patientInfo.BuildingName = lastBuildingName;
						patientInfo.FloorId = lastFloorId ?? 0;
						patientInfo.FloorName = lastFloorName;
						patientInfo.RoomId = lastRoomId ?? 0;
						patientInfo.RoomName = lastRoomName;
					}

					return patientInfo;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}



		public async Task<List<PatientInfoVm>> GetPatientInfoList()
		{
			try
			{
				using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
				{
					var sqlQuery = $@"
                                    SELECT 
									    Subquery.*
									FROM 
									    (
									    SELECT 
									        P.*, 
									        PVH.*, 
									        PVHD.*,
									        ROW_NUMBER() OVER (PARTITION BY PVH.""Id"" ORDER BY PVHD.""CreatedAt"" DESC) AS ""RowNum""
									    FROM 
									        public.""Patients"" AS P
									    JOIN 
									        public.""PatientVisitHistory"" AS PVH ON P.""Id"" = PVH.""PatientId""
									    JOIN 
									        public.""PatientVisitHistoryDetailLists"" AS PVHD ON PVH.""Id"" = PVHD.""PatientVisitHistoryId""
									    WHERE 
									        PVH.""PatientCensusStatus"" = {Convert.ToInt32(CensusStatus.ADMITTED)}
									    ) AS Subquery
									WHERE 
									    Subquery.""RowNum"" = 1;";
					await connection.OpenAsync();
					List<PatientInfoVm> patientInfoList = new();
					using (var command = new NpgsqlCommand(sqlQuery, connection))
					{
						using (var reader = await command.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								var firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? "" : reader.GetString(reader.GetOrdinal("FirstName"));
								var middleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? "" : reader.GetString(reader.GetOrdinal("MiddleName"));
								var lastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? "" : reader.GetString(reader.GetOrdinal("LastName"));
								var dateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? DateOnly.MinValue : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth")));

								var patientInfo = new PatientInfoVm
								{
									Id = reader.GetInt32(reader.GetOrdinal("Id")),
									DateOfBirth = dateOfBirth,
									FirstName = firstName,
									MiddleName = middleName,
									LastName = lastName,
									FullName = $"{firstName} {middleName} {lastName}".Trim(),
									MRN = reader.IsDBNull(reader.GetOrdinal("MRN")) ? "" : reader.GetString(reader.GetOrdinal("MRN")),
									HospitalId = reader.IsDBNull(reader.GetOrdinal("HospitalId")) ? 0 : reader.GetInt32(reader.GetOrdinal("HospitalId")),
									BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? 0 : reader.GetInt32(reader.GetOrdinal("BuildingId")),
									FloorId = reader.IsDBNull(reader.GetOrdinal("FloorId")) ? 0 : reader.GetInt32(reader.GetOrdinal("FloorId")),
									RoomId = reader.IsDBNull(reader.GetOrdinal("RoomId")) ? 0 : reader.GetInt32(reader.GetOrdinal("RoomId")),
									AdmissionDate = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
								};
								patientInfoList.Add(patientInfo);
							}
						}
					}
					return patientInfoList;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<Model.PatientVisitHistory> UpdatePatientUserStatusAsync(Model.PatientVisitHistory model)
		{
			var Patient = _MedChgContext.PatientVisitHistory.Where(x => x.Id == model.Id).FirstOrDefault();
			try
			{
				if (Patient != null)
				{
					Patient.UserId = model.UserId;
					Patient.UpdatedById = model.UpdatedById;
					Patient.UpdatedAt = TimeHelper.GetTexasTime();
					_MedChgContext.SaveChanges();
				}
			}
			catch (Exception)
			{
				throw;
			}
			return Patient;
		}

		public async Task<Patient?> GetPatientById(int patientId)
		{
			try
			{
				var Patient = await _MedChgContext.Patients.Where(x => x.Id == patientId).FirstOrDefaultAsync();
				var PatientVisit = await _MedChgContext.PatientVisitHistory.Where(x => x.PatientId == Patient.Id).OrderBy(x => x.Id).LastOrDefaultAsync();
				var PatientVisitDetail = await _MedChgContext.PatientVisitHistoryDetailLists.Where(x => x.PatientVisitHistoryId == PatientVisit.Id).FirstOrDefaultAsync();
				if (PatientVisit != null)
				{
					Patient.PatientVisitHistories.Id = PatientVisit.Id;
					Patient.PatientVisitHistories.UserId = PatientVisit.UserId;
					Patient.PatientVisitHistories.PatientVisitHistoryDetailLists.Add(PatientVisitDetail);
					Patient.PatientVisitHistories.PatientCensusStatus = PatientVisit.PatientCensusStatus;
					return Patient;
				}
				return null;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<bool> UpdatePatient(Patient model)
		{
			//Check Patient MRN From Same Hospital To Be Different
			var Patients = _MedChgContext.Patients.Where(x => x.Id != model.Id && x.MRN == model.MRN).ToList();
			foreach (var item in Patients)
			{
				var PatientVisitHistory = _MedChgContext.PatientVisitHistory.Where(x => x.PatientId == item.Id)
					.FirstOrDefault();
				var PatientVisitHistoryDetail = _MedChgContext.PatientVisitHistoryDetailLists
					.Where(x => x.PatientVisitHistoryId == PatientVisitHistory.Id)
					.FirstOrDefault();
				var CheckPVHId = _MedChgContext.PatientVisitHistoryDetailLists.
					Where(x => x.PatientVisitHistoryId == model.PatientVisitHistories.Id)
					.FirstOrDefault();
				if (PatientVisitHistoryDetail.HospitalId == CheckPVHId.HospitalId)
				{
					return false;
				}
			}
			var Patient = _MedChgContext.Patients.Where(x => x.Id == model.Id).FirstOrDefault();
			var UpdatePatientVisitHistory = _MedChgContext.PatientVisitHistory.Where(x => x.PatientId == model.Id)
					.FirstOrDefault();
			// Proceed with updating the patient details if no conflicts
			if (Patient != null)
			{
				Patient.MRN = model.MRN;
				Patient.FirstName = model.FirstName;
				Patient.LastName = model.LastName;
				Patient.MiddleName = model.MiddleName;
				Patient.Salutation = model.Salutation;
				Patient.Gender = model.Gender;
				Patient.DateOfBirth = model.DateOfBirth;
				Patient.PatientAddmissionDateTime = model.PatientAddmissionDateTime;
				UpdatePatientVisitHistory.PatientAddmissionDate = model.PatientAddmissionDateTime;
				UpdatePatientVisitHistory.PatientAddmissionTime = model.PatientAddmissionDateTime;
				Patient.UpdatedAt = TimeHelper.GetTexasTime();
				Patient.UpdatedById = model.UpdatedById;
				await _MedChgContext.SaveChangesAsync();
				return true;
			}

			return false;
		}

		//public async Task<Patient?> GetPatientVisitHistoryByMRN(string mrn)
		//{
		//    try
		//    {
		//        using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
		//        {
		//            Patient Patients = new Patient { };
		//            string query = @$"
		//                            SELECT pvh.*, pv.""HospitalId"",p.""FirstName"",p.""MiddleName"",p.""LastName"",p.""MRN"",p.""DateOfBirth""
		//                            FROM ""Patients"" p
		//                            LEFT JOIN ""PatientVisitHistory"" pvh ON p.""Id"" = pvh.""PatientId""
		//                            LEFT JOIN ""PatientVisits"" pv ON p.""Id"" = pv.""PatientId""
		//                            WHERE p.""MRN"" = '{mrn}'AND pv.""HospitalId"" IS NOT NULL AND pvh.""UserId"" IS NOT NULL;";
		//            NpgsqlCommand command = new NpgsqlCommand(query, connection);
		//            await connection.OpenAsync();
		//            NpgsqlDataReader reader = await command.ExecuteReaderAsync();
		//            while (reader.Read())
		//            {
		//                Patients.FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName"));
		//                Patients.MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName"));
		//                Patients.LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName"));
		//                Patients.MRN = reader.IsDBNull(reader.GetOrdinal("MRN")) ? null : reader.GetString(reader.GetOrdinal("MRN"));
		//                Patients.DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth")));
		//                Patients.PatientVisit.HospitalId = reader.IsDBNull(reader.GetOrdinal("HospitalId")) ? 0 : reader.GetInt16(reader.GetOrdinal("HospitalId"));
		//                PatientVisitHistoryDetailList pvh = new()
		//                {
		//                    PractitionerId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? 0 : reader.GetInt16(reader.GetOrdinal("UserId")),
		//                    PatientVisitHistoryId = reader.IsDBNull(reader.GetOrdinal("PatientId")) ? 0 : reader.GetInt16(reader.GetOrdinal("PatientId")),
		//                    CodeType = reader.IsDBNull(reader.GetOrdinal("Type")) ? null : reader.GetString(reader.GetOrdinal("Type")),
		//                    CodeName = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
		//                    PractitionerName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? null : reader.GetString(reader.GetOrdinal("UserName")),
		//                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.Today : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
		//                };
		//                Patients.VisitHistories.Add(pvh);
		//            }
		//            reader.Close();
		//            return Patients;
		//        }
		//    }
		//    catch (Exception)
		//    {
		//        return null;
		//        throw;
		//    }
		//}

		public async Task<Patient?> GetPatientByMRN(string mrn, int hospitalId)
		{
			try
			{
				using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
				{
					var sqlQuery = $@"
                       SELECT 
                                P.*,P.""Id"" as PatientId ,
                                PVH.*, 
                                PVHD.*,
                                PVHD.""HospitalId"" AS HospId,PVH.""UserId"" AS UId
                            FROM 
                                ""Patients"" P
                            JOIN 
                                ""PatientVisitHistory"" PVH ON P.""Id"" = PVH.""PatientId""
                            JOIN 
                                ""PatientVisitHistoryDetailLists"" PVHD ON PVH.""Id"" = PVHD.""PatientVisitHistoryId""
                            WHERE 
                                P.""MRN"" = '{mrn}'
                                AND PVHD.""HospitalId"" = {hospitalId};";
					await connection.OpenAsync();
					Patient PatientInfo = new();
					using (var command = new NpgsqlCommand(sqlQuery, connection))
					{
						using (var reader = await command.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								PatientInfo.Id = reader.GetInt32(reader.GetOrdinal("Id"));
								PatientInfo.MRN = reader.GetString(reader.GetOrdinal("MRN"));
								PatientInfo.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
								PatientInfo.MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ?
								"" :
								reader.GetString(reader.GetOrdinal("MiddleName"));
								PatientInfo.LastName = reader.GetString(reader.GetOrdinal("LastName"));
								PatientInfo.Gender = (Gender)reader.GetInt32(reader.GetOrdinal("Gender"));
								PatientInfo.Salutation = reader.GetString(reader.GetOrdinal("Salutation"));
								PatientInfo.DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth")));
								PatientVisitHistoryDetailList PatientVisitHistoryDetailLists = new();
								PatientVisitHistoryDetailLists.HospitalId = reader.GetInt32(reader.GetOrdinal("HospId"));
								PatientInfo.PatientVisitHistories.PatientVisitHistoryDetailLists.Add(PatientVisitHistoryDetailLists);
								PatientInfo.PatientVisitHistories.UserId = reader.GetInt32(reader.GetOrdinal("UId"));
								return PatientInfo;
							}
						}
					}
					return null;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}
		public async Task<Patient?> GetAdmittedPatientByMRN(string mrn)
		{
			try
			{
				var Patient = await _MedChgContext.Patients.Where(x => x.MRN == mrn).FirstOrDefaultAsync();
				var PatientVisit = await _MedChgContext.PatientVisitHistory.Where(x => x.PatientId == Patient.Id && x.PatientCensusStatus == CensusStatus.ADMITTED).FirstOrDefaultAsync();
				var PatientVisitDetail = await _MedChgContext.PatientVisitHistoryDetailLists.Where(x => x.PatientVisitHistoryId == PatientVisit.Id).FirstOrDefaultAsync();

				if (PatientVisit != null)
				{
					Patient.PatientVisitHistories.UserId = PatientVisit.UserId;
					Patient.PatientVisitHistories.PatientVisitHistoryDetailLists.Add(PatientVisitDetail);
					return Patient;
				}
				return null;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<Patient?> GetPatientByMRN(string mrn)
		{
			try
			{
				var Patient = await _MedChgContext.Patients.Where(x => x.MRN == mrn).FirstOrDefaultAsync();
				var PatientVisit = await _MedChgContext.PatientVisitHistory.Where(x => x.PatientId == Patient.Id && x.PatientCensusStatus != CensusStatus.ADMITTED).FirstOrDefaultAsync();
				var PatientVisitDetail = await _MedChgContext.PatientVisitHistoryDetailLists.Where(x => x.PatientVisitHistoryId == PatientVisit.Id).FirstOrDefaultAsync();
				if (PatientVisit != null)
				{
					Patient.PatientVisitHistories.UserId = PatientVisit.UserId;
					Patient.PatientVisitHistories.PatientVisitHistoryDetailLists.Add(PatientVisitDetail);
					return Patient;
				}
				return null;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<bool> CheckSameMRNAndNameInHospital(PatientDTO patient)
		{
			try
			{
				var PatientList = await _MedChgContext.Patients.Where(x => x.FirstName == patient.FirstName && x.MiddleName == patient.MiddleName && x.LastName == patient.LastName).ToListAsync();
				foreach (var item in PatientList)
				{
					var CheckPatient = await _MedChgContext.PatientVisitHistory.Where(x => x.PatientId == item.Id).FirstOrDefaultAsync();
					var CheckPatientDetail = await _MedChgContext.PatientVisitHistoryDetailLists.Where(x => x.PatientVisitHistoryId == CheckPatient.Id && x.HospitalId == patient.HospitalId).FirstOrDefaultAsync();
					if (CheckPatientDetail != null)
					{
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<bool> UpdateRegisteredPatient(PatientDTO model)
		{
			try
			{
				var existingPatient = await _MedChgContext.Patients
					.FirstOrDefaultAsync(x => x.Id == model.PateintId);
				if (existingPatient == null) return false;

				existingPatient.MRN = model.MRNNumber;
				existingPatient.FirstName = model.FirstName;
				existingPatient.MiddleName = model.MiddleName;
				existingPatient.LastName = model.LastName;
				existingPatient.Salutation = model.Salutation;
				existingPatient.Gender = model.Gender.Value;
				existingPatient.DateOfBirth = model.DOB.Value;
				existingPatient.UpdatedById = model.CreatedUserId;
				existingPatient.UpdatedAt = TimeHelper.GetTexasTime();
				existingPatient.PatientAddmissionDateTime = model.PatientAddmissionDateTime;
				var PatientVisitHistory = CreatePatientVisitHistory(model, model.PateintId.Value);
				await _MedChgContext.PatientVisitHistory.AddAsync(PatientVisitHistory);
				await _MedChgContext.SaveChangesAsync();
				var PatientVisitHistoryDetailList = CreatePatientVisitHistoryDetail(model, PatientVisitHistory.Id);
				await _MedChgContext.PatientVisitHistoryDetailLists.AddAsync(PatientVisitHistoryDetailList);
				await _MedChgContext.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		Model.PatientVisitHistory CreatePatientVisitHistory(PatientDTO model, int PateintId)
		{
			var PatientVisitHistory = new Model.PatientVisitHistory()
			{
				PatientId = PateintId,
				PatientAge = DateTime.Today.Year - model.DOB.Value.Year,

				ProviderId = model.ProviderId,
				UserId = model.PractitionerId,

				PatientAddmissionDate = model.PatientAddmissionDateTime,
				PatientAddmissionTime = TimeHelper.GetTexasTime(),
				CreatedById = model.CreatedUserId,

				PatientCensusStatus = CensusStatus.ADMITTED,
				Device = model.Device,
			};
			return PatientVisitHistory;
		}
		PatientVisitHistoryDetailList CreatePatientVisitHistoryDetail(PatientDTO model, int PatientVisitId)
		{
			var PatientVisitHistoryDetailList = new PatientVisitHistoryDetailList()
			{
				RoomId = model.RoomId.Value,
				FloorId = model.FloorId.Value,
				BuildingId = model.BuildingId.Value,
				HospitalId = model.HospitalId.Value,

				PractitionerId = model.PractitionerId,
				PractitionerName = model.PractitionerName,

				PatientVisitHistoryId = PatientVisitId,
				CreatedById = model.CreatedUserId,
				Device = model.Device,
			};
			return PatientVisitHistoryDetailList;
		}
		public async Task AddPatientAsync(PatientDTO model)
		{
			try
			{
				var Pateint = new Patient()
				{
					MRN = model.MRNNumber,
					FirstName = model.FirstName,
					MiddleName = model.MiddleName,
					LastName = model.LastName,
					Salutation = model.Salutation,
					DateOfBirth = model.DOB.Value,

					Gender = model.Gender.Value,
					CreatedById = model.CreatedUserId,
					PatientAddmissionDateTime = model.PatientAddmissionDateTime
				};
				await _MedChgContext.Patients.AddAsync(Pateint);
				await _MedChgContext.SaveChangesAsync();
				var PatientVisitHistory = CreatePatientVisitHistory(model, Pateint.Id);
				await _MedChgContext.PatientVisitHistory.AddAsync(PatientVisitHistory);
				await _MedChgContext.SaveChangesAsync();
				var PatientVisitHistoryDetailList = CreatePatientVisitHistoryDetail(model, PatientVisitHistory.Id);
				await _MedChgContext.PatientVisitHistoryDetailLists.AddAsync(PatientVisitHistoryDetailList);
				await _MedChgContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		public async Task<bool> UpdatePatientCensusStatus(int patientId, int hospitalId, int UpdatedById)
		{
			try
			{
				//var patient = await _MedChgContext.Patients
				//    .Include(x => x.PatientVisits)
				//    .Where(x => x.Id == patientId && x.PatientVisits.Any(s => s.HospitalId == hospitalId))
				//    .FirstOrDefaultAsync();

				//if (patient != null)
				//{
				//    var patientVisit = patient.PatientVisits.FirstOrDefault();
				//    if (patientVisit != null)
				//    {
				//        patientVisit.CensusStatus = false;
				//        patientVisit.UpdatedAt = TimeHelper.GetTexasTime();
				//        patientVisit.UpdatedById = UpdatedById;
				//        await _MedChgContext.SaveChangesAsync();
				//        return true;
				//    }
				return false;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task<Patient> GetPatientHistory(int providerId, Patient Model)
		{
			var PVH = new Patient();
			try
			{
				if (providerId > 0 && !string.IsNullOrEmpty(Model.MRN))
				{
					using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
					{
						var sqlQuery = $@"
                        Select PVH.*,
                                 PVHDL.""CodeName"" as CN,PVHDL.""CodeDescription"" as CD,PVHDL.""CodeType"" as CT,
                                 PVHDL.""CodeSubType"" as CST,PVHDL.""PractitionerName"" as PN,PVHDL.""CreatedAt"" as CA,
								 PVHDL.""Id"" as PVHDLID,
								  ""p"".""FirstName"" as PatientFirstName,
								  ""p"".""MiddleName"" as PatientMiddleName,
								  ""p"".""LastName"" as PatientLastName,
								  ""p"".""DateOfBirth"" as PatientDateOfBirth,
								  ""p"".""MRN"" as PatientMRN
                                FROM 
                                    ""Patients"" ""p""
                                JOIN 
                                    ""PatientVisitHistory"" PVH ON ""p"".""Id"" = PVH.""PatientId""
                                LEFT JOIN 
                                    ""PatientVisitHistoryDetailLists"" PVHDL ON PVH.""Id"" = PVHDL.""PatientVisitHistoryId""
                                WHERE 
                                    ""p"".""MRN"" = '{Model.MRN}'
                                    AND ""p"".""IsActive"" = 1
                                    AND (PVH.""IsActive"" = 1 OR PVH.""IsActive"" IS NULL)
                                    AND PVH.""PatientCensusStatus"" = 1
                                    AND PVHDL.""IsActive"" = 1
                                    AND PVH.""ProviderId"" = {providerId}
                                    AND PVHDL.""CodeName"" IS NOT NULL";

						// Add conditions for name and dob if provided
						if (!string.IsNullOrEmpty(Model.FirstName))
						{
							sqlQuery += $" AND (\"p\".\"FirstName\" ILIKE '{Model.FirstName}' AND COALESCE(\"p\".\"MiddleName\", '{Model.MiddleName}') ILIKE '' AND \"p\".\"LastName\" ILIKE '{Model.LastName}')";
						}
						sqlQuery += $" AND \"p\".\"DateOfBirth\" = '{Model.DateOfBirth.ToString("yyyy-MM-dd")}'";
						await connection.OpenAsync();
						using (var command = new NpgsqlCommand(sqlQuery, connection))
						{

							using (var reader = await command.ExecuteReaderAsync())
							{
								while (await reader.ReadAsync())
								{
									PVH.FirstName = reader.IsDBNull(reader.GetOrdinal("PatientFirstName")) ? string.Empty : reader.GetString(reader.GetOrdinal("PatientFirstName"));
									PVH.MiddleName = reader.IsDBNull(reader.GetOrdinal("PatientMiddleName")) ? string.Empty : reader.GetString(reader.GetOrdinal("PatientMiddleName"));
									PVH.LastName = reader.IsDBNull(reader.GetOrdinal("PatientLastName")) ? string.Empty : reader.GetString(reader.GetOrdinal("PatientLastName"));
									PVH.DateOfBirth = reader.IsDBNull(reader.GetOrdinal("PatientDateOfBirth")) ? DateOnly.MinValue : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("PatientDateOfBirth")));
									PVH.MRN = reader.IsDBNull(reader.GetOrdinal("PatientMRN")) ? string.Empty : reader.GetString(reader.GetOrdinal("PatientMRN"));
									PVH.PatientVisitHistories.Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id"));
									PVH.PatientVisitHistories.PatientId = reader.IsDBNull(reader.GetOrdinal("PatientId")) ? 0 : reader.GetInt32(reader.GetOrdinal("PatientId"));
									PVH.PatientVisitHistories.IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive"));
									PVH.PatientVisitHistories.CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
									PVH.PatientVisitHistories.Date = reader.IsDBNull(reader.GetOrdinal("Date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Date"));
									PVH.PatientVisitHistories.CreatedById = reader.IsDBNull(reader.GetOrdinal("CreatedById")) ? 0 : reader.GetInt32(reader.GetOrdinal("CreatedById"));
									PVH.PatientVisitHistories.PatientAddmissionDate = reader.IsDBNull(reader.GetOrdinal("PatientAddmissionDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientAddmissionDate"));
									PVH.PatientVisitHistories.PatientAge = reader.IsDBNull(reader.GetOrdinal("PatientAge")) ? 0 : reader.GetInt32(reader.GetOrdinal("PatientAge"));
									var PVHDL = new PatientVisitHistoryDetailList
									{
										Id = reader.IsDBNull(reader.GetOrdinal("PVHDLID")) ? 0 : reader.GetInt32(reader.GetOrdinal("PVHDLID")),
										CodeName = reader.IsDBNull(reader.GetOrdinal("CN")) ? string.Empty : reader.GetString(reader.GetOrdinal("CN")),
										CodeDescription = reader.IsDBNull(reader.GetOrdinal("CD")) ? string.Empty : reader.GetString(reader.GetOrdinal("CD")),
										CodeType = reader.IsDBNull(reader.GetOrdinal("CT")) ? string.Empty : reader.GetString(reader.GetOrdinal("CT")),
										PractitionerName = reader.IsDBNull(reader.GetOrdinal("PN")) ? string.Empty : reader.GetString(reader.GetOrdinal("PN")),
										CodeSubType = reader.IsDBNull(reader.GetOrdinal("CST")) ? string.Empty : reader.GetString(reader.GetOrdinal("CST")),
										CreatedAt = reader.IsDBNull(reader.GetOrdinal("CA")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CA"))
									};

									PVH.PatientVisitHistories.PatientVisitHistoryDetailLists.Add(PVHDL);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Optionally log the exception
				Console.WriteLine($"Error: {ex.Message}");
			}
			return PVH;
		}

		public async Task<List<Patient>> GetAllDischargePatientsByProviderId(int Id)
		{
			try
			{
				var PatientList = new List<Patient>();
				using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
				{
					var sqlQuery = $@"
                        SELECT 
                            P.*
                            FROM 
                                ""Patients"" AS P
                            INNER JOIN 
                                ""PatientVisitHistory"" AS PVH
                            ON 
                                P.""Id"" = PVH.""PatientId""
                            WHERE PVH.""PatientCensusStatus"" = {Convert.ToInt32(CensusStatus.DISCHARGED)} AND PVH.""ProviderId"" = {Id};";
					await connection.OpenAsync();
					using (var command = new NpgsqlCommand(sqlQuery, connection))
					{
						using (var reader = await command.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								var firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? "" : reader.GetString(reader.GetOrdinal("FirstName"));
								var middleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? "" : reader.GetString(reader.GetOrdinal("MiddleName"));
								var lastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? "" : reader.GetString(reader.GetOrdinal("LastName"));
								var dateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? DateOnly.MinValue : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth")));
								var patientInfo = new Patient
								{
									Id = reader.GetInt32(reader.GetOrdinal("Id")),
									DateOfBirth = dateOfBirth,
									FirstName = firstName,
									MiddleName = middleName,
									LastName = lastName,
									Salutation = reader.IsDBNull(reader.GetOrdinal("Salutation")) ? "" : reader.GetString(reader.GetOrdinal("Salutation")),
									Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? 0 : reader.GetInt32(reader.GetOrdinal("Gender")) == 1 ? Gender.Male : Gender.Female,
									MRN = reader.IsDBNull(reader.GetOrdinal("MRN")) ? "" : reader.GetString(reader.GetOrdinal("MRN")),
									PatientAddmissionDateTime = reader.IsDBNull(reader.GetOrdinal("PatientAddmissionDateTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientAddmissionDateTime")),
								};
								PatientList.Add(patientInfo);
							}
						}
					}
					return PatientList;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<string> GetPatientCensusStatusAsync(string MRN, int HospitalId)
		{
			try
			{
				var PatientList = new List<Patient>();
				using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
				{
					var sqlQuery = $@"
									SELECT visit.""PatientCensusStatus"" as Status
									FROM ""Patients"" patient
									JOIN ""PatientVisitHistory"" visit ON patient.""Id"" = visit.""PatientId""
									JOIN ""PatientVisitHistoryDetailLists"" visitDetail ON visit.""Id"" = visitDetail.""PatientVisitHistoryId""
									WHERE patient.""MRN"" = @MRN AND visitDetail.""HospitalId"" = @HospitalId";
					await connection.OpenAsync();
					using (var command = new NpgsqlCommand(sqlQuery, connection))
					{
						command.Parameters.AddWithValue("@MRN", MRN);
						command.Parameters.AddWithValue("@HospitalId", HospitalId);
						using (var reader = await command.ExecuteReaderAsync())
						{
							var Result = reader.IsDBNull(reader.GetOrdinal("Status")) ? 0 : reader.GetInt32(reader.GetOrdinal("Status"));
							return Result.ToString();
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}

}
