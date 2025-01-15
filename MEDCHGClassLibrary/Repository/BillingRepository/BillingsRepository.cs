using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Services;
using MEDCHGClassLibrary.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace MEDCHGClassLibrary.Repository.BillingRepository
{
	public class BillingsRepository : IBillingRepository
	{
		private string _npgsqlDataSource;

		public string NpgsqlDataSource
		{
			get { return _npgsqlDataSource; }
			set { _npgsqlDataSource = value; }
		}

		private IConfiguration _configureOptions;
		private ResponseMessage ResponseMessage;

		private readonly MedChgContext _MedChgContext;
		public BillingsRepository(MedChgContext medChgContext, IConfiguration configureOptions, ResponseMessage responseMessage)
		{
			_MedChgContext = medChgContext;
			_configureOptions = configureOptions;
			_npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection");
			ResponseMessage = responseMessage;
		}
		public async Task Update(int Id, Model.PatientVisitHistory Model)
		{
			var PatientVisitHistory = await _MedChgContext.PatientVisitHistory.Where(x => x.PatientId == Id && x.Id == Model.Id).FirstOrDefaultAsync();
			if (PatientVisitHistory is not null)
			{
				PatientVisitHistory.UpdatedById = Model.UpdatedById;
				PatientVisitHistory.UpdatedAt = TimeHelper.GetTexasTime();
				PatientVisitHistory.PatientCensusStatus = Model.PatientCensusStatus;
				switch (Model.PatientCensusStatus)
				{
					case CensusStatus.ADMITTED:
						await _MedChgContext.PatientVisitHistoryDetailLists.Where(x => x.PatientVisitHistoryId == PatientVisitHistory.Id)
			  .ExecuteUpdateAsync(setters => setters
			  .SetProperty(b => b.UpdatedAt, TimeHelper.GetTexasTime())
			  .SetProperty(b => b.UpdatedById, Model.UpdatedById));
						break;
					case CensusStatus.DISCHARGED:
						PatientVisitHistory.PatientDischargeById = Model.UpdatedById;
						PatientVisitHistory.PatientDischargeDate = TimeHelper.GetTexasTime();
						await _MedChgContext.PatientVisitHistoryDetailLists.Where(x => x.PatientVisitHistoryId == PatientVisitHistory.Id)
			  .ExecuteUpdateAsync(setters => setters
			  .SetProperty(b => b.UpdatedAt, TimeHelper.GetTexasTime())
			  .SetProperty(b => b.UpdatedById, Model.UpdatedById));
						break;
					case CensusStatus.BILLED:
						PatientVisitHistory.PatientBilledById = Model.UpdatedById;
						PatientVisitHistory.PatientBilledDate = TimeHelper.GetTexasTime();
						await _MedChgContext.PatientVisitHistoryDetailLists.Where(x => x.PatientVisitHistoryId == PatientVisitHistory.Id)
			  .ExecuteUpdateAsync(setters => setters
			  .SetProperty(b => b.UpdatedAt, TimeHelper.GetTexasTime())
			  .SetProperty(b => b.UpdatedById, Model.UpdatedById));
						break;
				}
				await _MedChgContext.SaveChangesAsync();
			}
		}

		public async Task UpdateHistory(Model.PatientVisitHistory Model)
		{
			var PatientVisitHistoryDetailList = await _MedChgContext.PatientVisitHistoryDetailLists
				.FirstOrDefaultAsync(x => x.Id == Model.PatientVisitHistoryDetailLists.FirstOrDefault().Id);
			if (PatientVisitHistoryDetailList is not null)
			{
				if (Model.PatientCensusStatus == CensusStatus.ADMITTED)
				{
					PatientVisitHistoryDetailList.CodeName = Model.PatientVisitHistoryDetailLists.FirstOrDefault().CodeName;
					PatientVisitHistoryDetailList.CodeDescription = Model.PatientVisitHistoryDetailLists.FirstOrDefault().CodeDescription;
					PatientVisitHistoryDetailList.CodeType = Model.PatientVisitHistoryDetailLists.FirstOrDefault().CodeType;
					PatientVisitHistoryDetailList.CodeSubType = Model.PatientVisitHistoryDetailLists.FirstOrDefault().CodeSubType;
					PatientVisitHistoryDetailList.UpdatedById = Model.PatientVisitHistoryDetailLists.FirstOrDefault().UpdatedById;
					PatientVisitHistoryDetailList.UpdatedAt = TimeHelper.GetTexasTime();
					if (Model.PatientVisitHistoryDetailLists.FirstOrDefault().CreatedAt != DateTime.MaxValue)
					{
						PatientVisitHistoryDetailList.CreatedAt = Model.PatientVisitHistoryDetailLists.FirstOrDefault().CreatedAt;
						PatientVisitHistoryDetailList.Date = Model.PatientVisitHistoryDetailLists.FirstOrDefault().CreatedAt;
					}
					PatientVisitHistoryDetailList.Device = Model.Device;
				}
				else if (Model.PatientCensusStatus == CensusStatus.DISCHARGED)
				{
					PatientVisitHistoryDetailList.IsActive = Model.PatientVisitHistoryDetailLists.FirstOrDefault().IsActive;
					PatientVisitHistoryDetailList.HospitalId = Model.PatientVisitHistoryDetailLists.FirstOrDefault().HospitalId;
					PatientVisitHistoryDetailList.BuildingId = Model.PatientVisitHistoryDetailLists.FirstOrDefault().BuildingId;
					PatientVisitHistoryDetailList.FloorId = Model.PatientVisitHistoryDetailLists.FirstOrDefault().FloorId;
					PatientVisitHistoryDetailList.RoomId = Model.PatientVisitHistoryDetailLists.FirstOrDefault().RoomId;
					PatientVisitHistoryDetailList.Date = Model.PatientVisitHistoryDetailLists.FirstOrDefault().Date;
					PatientVisitHistoryDetailList.CodeName = Model.PatientVisitHistoryDetailLists.FirstOrDefault().CodeName;
					PatientVisitHistoryDetailList.CodeDescription = Model.PatientVisitHistoryDetailLists.FirstOrDefault().CodeDescription;
					PatientVisitHistoryDetailList.CodeType = Model.PatientVisitHistoryDetailLists.FirstOrDefault().CodeType;
					PatientVisitHistoryDetailList.CodeSubType = Model.PatientVisitHistoryDetailLists.FirstOrDefault().CodeSubType;
					PatientVisitHistoryDetailList.PractitionerId = Model.PatientVisitHistoryDetailLists.FirstOrDefault().PractitionerId;
					PatientVisitHistoryDetailList.PractitionerName = Model.PatientVisitHistoryDetailLists.FirstOrDefault().PractitionerName;
					PatientVisitHistoryDetailList.UpdatedById = Model.PatientVisitHistoryDetailLists.FirstOrDefault().UpdatedById;
					PatientVisitHistoryDetailList.UpdatedAt = TimeHelper.GetTexasTime();
					PatientVisitHistoryDetailList.Device = Model.PatientVisitHistoryDetailLists.FirstOrDefault().Device;
					PatientVisitHistoryDetailList.Note = Model.PatientVisitHistoryDetailLists.FirstOrDefault().Note;
				}
				else
				{
					//No Operation Will Be Done In Case Of Billed
				}
				await _MedChgContext.SaveChangesAsync();
			}
		}

		public async Task<List<Model.PatientVisitHistory>> GetAllPatientVisitHistory()
		{
			try
			{
				var PVHList = new List<Model.PatientVisitHistory>();
				using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
				{
					var sqlQuery = $@"
                        SELECT 
                            PVH.*
                            FROM ""PatientVisitHistory"" AS PVH
                            WHERE PVH.""PatientCensusStatus""= {Convert.ToInt32(CensusStatus.DISCHARGED)};
                    ";
					await connection.OpenAsync();
					using (var command = new NpgsqlCommand(sqlQuery, connection))
					{

						using (var reader = await command.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								var PVH = new Model.PatientVisitHistory();
								PVH.Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id"));
								PVH.PatientId = reader.IsDBNull(reader.GetOrdinal("PatientId")) ? 0 : reader.GetInt32(reader.GetOrdinal("PatientId"));
								PVH.IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive"));
								PVH.CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
								PVH.Date = reader.IsDBNull(reader.GetOrdinal("Date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Date"));
								PVH.CreatedById = reader.IsDBNull(reader.GetOrdinal("CreatedById")) ? 0 : reader.GetInt32(reader.GetOrdinal("CreatedById"));
								PVH.UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"));
								PVH.UpdatedById = reader.IsDBNull(reader.GetOrdinal("UpdatedById")) ? 0 : reader.GetInt32(reader.GetOrdinal("UpdatedById"));
								PVH.Device = reader.IsDBNull(reader.GetOrdinal("Device")) ? string.Empty : reader.GetString(reader.GetOrdinal("Device"));
								PVH.PatientAddmissionDate = reader.IsDBNull(reader.GetOrdinal("PatientAddmissionDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientAddmissionDate"));
								PVH.PatientAddmissionTime = reader.IsDBNull(reader.GetOrdinal("PatientAddmissionTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientAddmissionTime"));
								PVH.PatientAge = reader.IsDBNull(reader.GetOrdinal("PatientAge")) ? 0 : reader.GetInt32(reader.GetOrdinal("PatientAge"));
								PVH.ProviderId = reader.IsDBNull(reader.GetOrdinal("ProviderId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProviderId"));
								PVH.PatientDischargeDate = reader.IsDBNull(reader.GetOrdinal("PatientDischargeDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientDischargeDate"));
								PVH.PatientBilledDate = reader.IsDBNull(reader.GetOrdinal("PatientBilledDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientBilledDate"));
								PVHList.Add(PVH);
							}
						}
					}
					return PVHList;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<List<PatientVisitHistoryDetailList>> GetAllPatientVisitHistoryDetailList()
		{
			try
			{
				var PVHDLList = new List<PatientVisitHistoryDetailList>();
				using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
				{
					var sqlQuery = $@"
                        SELECT 
                            PVHDL.*
                            FROM ""PatientVisitHistoryDetailLists"" AS PVHDL
                            WHERE PVHDL.""IsActive"" = 1;
                           ";
					await connection.OpenAsync();
					using (var command = new NpgsqlCommand(sqlQuery, connection))
					{
						using (var reader = await command.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								var PVHDL = new PatientVisitHistoryDetailList()
								{
									HospitalId = reader.IsDBNull(reader.GetOrdinal("HospitalId")) ? 0 : reader.GetInt32(reader.GetOrdinal("HospitalId")),
									BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? 0 : reader.GetInt32(reader.GetOrdinal("BuildingId")),
									CodeDescription = reader.IsDBNull(reader.GetOrdinal("CodeDescription")) ? "" : reader.GetString(reader.GetOrdinal("CodeDescription")),
									CodeName = reader.IsDBNull(reader.GetOrdinal("CodeName")) ? "" : reader.GetString(reader.GetOrdinal("CodeName")),
									CodeSubType = reader.IsDBNull(reader.GetOrdinal("CodeSubType")) ? "" : reader.GetString(reader.GetOrdinal("CodeSubType")),
									CodeType = reader.IsDBNull(reader.GetOrdinal("CodeType")) ? "" : reader.GetString(reader.GetOrdinal("CodeType")),
									CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
									CreatedById = reader.IsDBNull(reader.GetOrdinal("CreatedById")) ? 0 : reader.GetInt32(reader.GetOrdinal("CreatedById")),
									Date = reader.IsDBNull(reader.GetOrdinal("Date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Date")),
									Device = reader.IsDBNull(reader.GetOrdinal("Device")) ? "" : reader.GetString(reader.GetOrdinal("Device")),
									FloorId = reader.IsDBNull(reader.GetOrdinal("FloorId")) ? 0 : reader.GetInt32(reader.GetOrdinal("FloorId")),
									Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
									IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive")),
									Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString(reader.GetOrdinal("Note")),
									PatientVisitHistoryId = reader.IsDBNull(reader.GetOrdinal("PatientVisitHistoryId")) ? 0 : reader.GetInt32(reader.GetOrdinal("PatientVisitHistoryId")),
									PractitionerId = reader.IsDBNull(reader.GetOrdinal("PractitionerId")) ? 0 : reader.GetInt32(reader.GetOrdinal("PractitionerId")),
									PractitionerName = reader.IsDBNull(reader.GetOrdinal("PractitionerName")) ? string.Empty : reader.GetString(reader.GetOrdinal("PractitionerName")),
									RoomId = reader.IsDBNull(reader.GetOrdinal("RoomId")) ? 0 : reader.GetInt32(reader.GetOrdinal("RoomId")),
									UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
									UpdatedById = reader.IsDBNull(reader.GetOrdinal("UpdatedById")) ? 0 : reader.GetInt32(reader.GetOrdinal("UpdatedById")),
								};
								PVHDLList.Add(PVHDL);
							}
						}
					}
					return PVHDLList;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task Add(PatientVisitHistoryDetailList Model)
		{
			try
			{
				await _MedChgContext.PatientVisitHistoryDetailLists.AddAsync(Model);
				await _MedChgContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error Message ", ex.ToString());
				throw;
			}
		}

		public async Task<PatientDTO> GetArchivePatientVisitHistory(int providerId, PatientDTO Model)
		{
			var PVH = new PatientDTO();
			try
			{
				if (providerId > 0 && !string.IsNullOrEmpty(Model.PatientObj.MRN))
				{
					using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
					{
						var sqlQuery = $@"
                        SELECT 
                            P.*,
                            PVH.*,
                            PVH.""Id"" As PVHId,
                            PVH.""CreatedAt"" As PVHCreatedAt,
                            PVH.""UpdatedAt"" As PVHUpdatedAt,
                            PVH.""UpdatedById"" As PVHUpdatedById,
                            PVH.""CreatedById"" As PVHCreatedById
                            FROM ""PatientVisitHistory"" AS PVH
                            JOIN 
                            ""Patients"" AS P ON P.""Id"" = PVH.""PatientId""
                            WHERE PVH.""PatientCensusStatus""= {Convert.ToInt32(CensusStatus.BILLED)}
                            AND P.""MRN"" = '{Model.PatientObj.MRN}' AND PVH.""ProviderId"" = {providerId}";
                        // Add conditions for name and dob if provided
                        if (!string.IsNullOrEmpty(Model.PatientObj.MiddleName))
                        {
                            sqlQuery += $" AND (\"p\".\"FirstName\" ILIKE '{Model.PatientObj.FirstName}' AND \"p\".\"MiddleName\" ILIKE '{Model.PatientObj.MiddleName}' AND \"p\".\"LastName\" ILIKE '{Model.PatientObj.LastName}')";
                        }
						else
						{
                            sqlQuery += $" AND (\"p\".\"FirstName\" ILIKE '{Model.PatientObj.FirstName}' AND \"p\".\"LastName\" ILIKE '{Model.PatientObj.LastName}')";
                        }
                        sqlQuery += $" AND \"p\".\"DateOfBirth\" = '{Model.PatientObj.DateOfBirth.ToString("yyyy-MM-dd")}'";
                        await connection.OpenAsync();
						using (var command = new NpgsqlCommand(sqlQuery, connection))
						{

							using (var reader = await command.ExecuteReaderAsync())
							{
								PVH.PatientVisitHistoryObj = new();
								PVH.PatientObj = new();
								PVH.PatientVisitHistoryDetailLists = new();
								while (await reader.ReadAsync())
								{
									PVH.PatientId = reader.GetInt32(reader.GetOrdinal("PatientId"));
									PVH.PatientObj.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
									PVH.PatientObj.MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? "" : reader.GetString(reader.GetOrdinal("MiddleName"));
									PVH.PatientObj.LastName = reader.GetString(reader.GetOrdinal("LastName"));
									PVH.PatientObj.MRN = reader.GetString(reader.GetOrdinal("MRN"));
									PVH.PatientObj.DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth")));
									PVH.PatientVisitHistoryObj.Id = reader.GetInt32(reader.GetOrdinal("PVHId"));
									PVH.PatientVisitHistoryObj.Date = reader.GetDateTime(reader.GetOrdinal("Date"));
									PVH.PatientVisitHistoryObj.CreatedById = reader.GetInt32(reader.GetOrdinal("PVHCreatedById"));
									PVH.PatientVisitHistoryObj.UpdatedAt = reader.GetDateTime(reader.GetOrdinal("PVHUpdatedAt"));
									PVH.PatientVisitHistoryObj.UpdatedById = reader.GetInt32(reader.GetOrdinal("PVHUpdatedById"));
									PVH.PatientVisitHistoryObj.PatientAddmissionDate = reader.GetDateTime(reader.GetOrdinal("PatientAddmissionDate"));
									PVH.PatientVisitHistoryObj.PatientAddmissionTime = reader.GetDateTime(reader.GetOrdinal("PatientAddmissionTime"));
									PVH.PatientVisitHistoryObj.PatientAge = reader.GetInt32(reader.GetOrdinal("PatientAge"));
									//PVH.PatientAge = CalCulateAge.AgeCalculator(DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("PatientDOB"))));
									PVH.PatientVisitHistoryObj.ProviderId = reader.GetInt32(reader.GetOrdinal("ProviderId"));
									PVH.PatientVisitHistoryObj.PatientDischargeDate = reader.IsDBNull(reader.GetOrdinal("PatientDischargeDate")) ? null : reader.GetDateTime(reader.GetOrdinal("PatientDischargeDate"));
									PVH.PatientVisitHistoryObj.PatientBilledDate = reader.IsDBNull(reader.GetOrdinal("PatientBilledDate")) ? null : reader.GetDateTime(reader.GetOrdinal("PatientBilledDate"));
									var DetailList = _MedChgContext.PatientVisitHistoryDetailLists.Where(x => x.PatientVisitHistoryId == PVH.PatientVisitHistoryObj.Id && x.IsActive == 1).ToList();
									PVH.PatientVisitHistoryDetailLists.AddRange(DetailList);
								}
							}
						}
					}


				}
			}
			catch (Exception ex)
			{

			}
			return PVH;
		}

		public async Task<List<Model.PatientVisitHistory>> GetBillingReportData(BillingReportModel model)
		{
			var PVHList = new List<Model.PatientVisitHistory>();
			try
			{
				if (model.ProviderId > 0)
				{
					using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
					{
						var sqlQuery = $@"
                        SELECT 
                            PVH.*
                            FROM ""PatientVisitHistory"" AS PVH
                            WHERE PVH.""PatientCensusStatus"" = {Convert.ToInt32(CensusStatus.DISCHARGED)}
                            AND PVH.""ProviderId"" = {model.ProviderId}
                    ";
						if (model.FromDate != null && model.FromDate.Value != null)
						{
							sqlQuery += $@" AND PVH.""PatientDischargeDate"" >= '{model.FromDate.Value.ToString("yyyy-MM-dd")}' ";
						}
						if (model.ToDate != null && model.ToDate.Value != null)
						{
							sqlQuery += $@" AND PVH.""PatientDischargeDate"" < '{model.ToDate.Value.AddDays(1).ToString("yyyy-MM-dd")}' ";
						}
						sqlQuery += ";";
						await connection.OpenAsync();
						using (var command = new NpgsqlCommand(sqlQuery, connection))
						{

							using (var reader = await command.ExecuteReaderAsync())
							{
								while (await reader.ReadAsync())
								{
									var PVH = new Model.PatientVisitHistory();
									PVH.Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id"));
									PVH.PatientId = reader.IsDBNull(reader.GetOrdinal("PatientId")) ? 0 : reader.GetInt32(reader.GetOrdinal("PatientId"));
									PVH.IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive"));
									PVH.CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
									PVH.Date = reader.IsDBNull(reader.GetOrdinal("Date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Date"));
									PVH.CreatedById = reader.IsDBNull(reader.GetOrdinal("CreatedById")) ? 0 : reader.GetInt32(reader.GetOrdinal("CreatedById"));
									PVH.UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"));
									PVH.UpdatedById = reader.IsDBNull(reader.GetOrdinal("UpdatedById")) ? 0 : reader.GetInt32(reader.GetOrdinal("UpdatedById"));
									PVH.Device = reader.IsDBNull(reader.GetOrdinal("Device")) ? string.Empty : reader.GetString(reader.GetOrdinal("Device"));
									PVH.PatientAddmissionDate = reader.IsDBNull(reader.GetOrdinal("PatientAddmissionDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientAddmissionDate"));
									PVH.PatientAddmissionTime = reader.IsDBNull(reader.GetOrdinal("PatientAddmissionTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientAddmissionTime"));
									PVH.PatientAge = reader.IsDBNull(reader.GetOrdinal("PatientAge")) ? 0 : reader.GetInt32(reader.GetOrdinal("PatientAge"));
									PVH.ProviderId = reader.IsDBNull(reader.GetOrdinal("ProviderId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProviderId"));
									PVH.PatientDischargeDate = reader.IsDBNull(reader.GetOrdinal("PatientDischargeDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientDischargeDate"));
									PVH.PatientBilledDate = reader.IsDBNull(reader.GetOrdinal("PatientBilledDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PatientBilledDate"));
									PVHList.Add(PVH);
								}
							}
						}
					}
					foreach (var item in PVHList)
					{
						item.PatientVisitHistoryDetailLists = _MedChgContext.PatientVisitHistoryDetailLists.Where(x => x.PatientVisitHistoryId == item.Id && x.IsActive == 1).OrderBy(x => x.Id).ToList();
					}
				}
			}
			catch (Exception ex)
			{

			}
			return PVHList;
		}

		public async Task<ResponseMessage> UpdatePatientInfo(Patient model)
		{
			// Fetch the patient visit history for the current patient
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

					// If both MRN and Hospital ID match, do not allow the update
					Console.WriteLine("Try Different MRN This MRN is in Used!");
					ResponseMessage.Success = false;
					ResponseMessage.Message = "Try Different MRN This MRN is in Used!";
					ResponseMessage.ErrorCode = "UPDATE_ERROR";
					return ResponseMessage;
				}
			}
			var Patient = _MedChgContext.Patients.Where(x => x.Id == model.Id).FirstOrDefault();
			// Proceed with updating the patient details if no conflicts

			try
			{
				var PatientVisitHistory = _MedChgContext.PatientVisitHistory.Where(x => x.Id == model.PatientVisitHistories.Id)
						.FirstOrDefault();
				PatientVisitHistory.PatientAddmissionDate = model.PatientVisitHistories.PatientAddmissionDate;
				PatientVisitHistory.PatientAddmissionTime = model.PatientVisitHistories.PatientAddmissionTime;
				Patient.FirstName = model.FirstName;
				Patient.MiddleName = model.MiddleName;
				Patient.LastName = model.LastName;
				Patient.MRN = model.MRN;
				Patient.DateOfBirth = model.DateOfBirth;
				Patient.Salutation = model.Salutation;
				Patient.PatientAddmissionDateTime = model.PatientAddmissionDateTime;
				Patient.Gender = (Gender)model.Gender;
				Patient.UpdatedAt = TimeHelper.GetTexasTime();
				Patient.UpdatedById = model.UpdatedById;
				await _MedChgContext.SaveChangesAsync();
				ResponseMessage.Success = true;
				ResponseMessage.Message = "Patient Updated successfully.";
				return ResponseMessage;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error Message ", ex.ToString());
				ResponseMessage.Success = false;
				ResponseMessage.Message = "An error occurred while Updating the patient.";
				ResponseMessage.ErrorCode = "UPDATE_ERROR";
				ResponseMessage.Errors.Add(ex.Message);
				throw;
			}
		}

		public async Task<PatientDTO> GetPatientLocation(int VisitId)
		{
			var Location = new PatientDTO();
			try
			{
				using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
				{
					var sqlQuery = $@"
                                    SELECT 
                                        PVH.*, 
                                        b.""Id"" AS BuildingId,    
                                        b.""BuildingName"" AS BuildingName,
                                        f.""Id"" AS FloorId,       
                                        f.""Name"" AS FloorName,   
                                        r.""Id"" AS RoomId,        
                                        r.""Name"" AS RoomName     
                                    FROM 
                                        ""PatientVisitHistory"" AS PVH
                                    JOIN 
                                        ""PatientVisitHistoryDetailLists"" AS PVHD ON PVH.""Id"" = PVHD.""PatientVisitHistoryId"" 
                                    JOIN 
                                        ""Buildings"" AS b ON b.""Id"" = PVHD.""BuildingId"" 
                                    JOIN 
                                        ""Floors"" AS f ON f.""Id"" = PVHD.""FloorId""        
                                    JOIN 
                                        ""Rooms"" AS r ON r.""Id"" = PVHD.""RoomId""
                                    WHERE 
                                        PVH.""PatientCensusStatus"" = {Convert.ToInt32(CensusStatus.DISCHARGED)}
                                        AND PVH.""Id"" = {VisitId};";

					await connection.OpenAsync();
					using (var command = new NpgsqlCommand(sqlQuery, connection))
					{
						using (var reader = await command.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								// Selectively retrieve columns
								Location.BuildingName = (string)reader["buildingname"];
								Location.RoomName = (string)reader["roomname"];
								Location.FloorName = (string)reader["floorname"];
							}
						}
					}
				}
			}
			catch (Exception ex)
			{

			}
			return Location;
		}

		public async Task<List<PatientDTO>> GetAllPatientLocation()
		{
			var LocationList = new List<PatientDTO>();
			try
			{
				using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
				{
					var sqlQuery = $@"
                                    SELECT 
                                        PVH.*, 
                                        b.""Id"" AS BuildingId,    
                                        b.""BuildingName"" AS BuildingName,
                                        f.""Id"" AS FloorId,       
                                        f.""Name"" AS FloorName,   
                                        r.""Id"" AS RoomId,        
                                        r.""Name"" AS RoomName     
                                    FROM 
                                        ""PatientVisitHistory"" AS PVH
                                    JOIN 
                                        ""PatientVisitHistoryDetailLists"" AS PVHD ON PVH.""Id"" = PVHD.""PatientVisitHistoryId"" 
                                    JOIN 
                                        ""Buildings"" AS b ON b.""Id"" = PVHD.""BuildingId"" 
                                    JOIN 
                                        ""Floors"" AS f ON f.""Id"" = PVHD.""FloorId""        
                                    JOIN 
                                        ""Rooms"" AS r ON r.""Id"" = PVHD.""RoomId""
                                    WHERE 
                                        PVH.""PatientCensusStatus"" = {Convert.ToInt32(CensusStatus.DISCHARGED)} AND PVHD.""IsActive"" = 1
                                        ;";

					await connection.OpenAsync();
					using (var command = new NpgsqlCommand(sqlQuery, connection))
					{

						using (var reader = await command.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								var Location = new PatientDTO();
								// Selectively retrieve columns
								Location.BuildingName = (string)reader["buildingname"];
								Location.RoomName = (string)reader["roomname"];
								Location.FloorName = (string)reader["floorname"];
								Location.BuildingId = (int)reader["buildingid"];
								Location.RoomId = (int)reader["roomid"];
								Location.FloorId = (int)reader["floorid"];
								LocationList.Add(Location);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{

			}
			return LocationList;
		}

	}
}
