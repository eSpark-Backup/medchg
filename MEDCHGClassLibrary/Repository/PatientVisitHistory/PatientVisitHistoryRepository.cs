using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MEDCHGClassLibrary.Repository.PatientVisitHistory
{
    public class PatientVisitHistoryRepository : IPatientVisitHistoryRespository
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
        public PatientVisitHistoryRepository(IConfiguration configureOptions, MedChgContext medChgContext, ResponseMessage responseMessage)
        {
            _configureOptions = configureOptions;
            _MedChgContext = medChgContext;
            _npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection");
            ResponseMessage = responseMessage;
        }
        public async Task AddVisitHistory(Model.PatientVisitHistory model)
        {
            try
            {
                var VisitHistory = new Model.PatientVisitHistory();
                VisitHistory.PatientId = model.PatientId;
                VisitHistory.PatientAge = model.PatientAge;
                VisitHistory.PatientAddmissionDate = model.CreatedAt;
                VisitHistory.PatientAddmissionTime = model.CreatedAt;
                VisitHistory.CreatedById = model.CreatedById;
                VisitHistory.Device = "Web";

                VisitHistory.PatientCensusStatus = model.PatientCensusStatus;
                VisitHistory.ProviderId = model.ProviderId;
                await _MedChgContext.PatientVisitHistory.AddAsync(VisitHistory);
                await _MedChgContext.SaveChangesAsync();
                var PatientVisitHistoryDetailList = new PatientVisitHistoryDetailList();
                PatientVisitHistoryDetailList = model.PatientVisitHistoryDetailLists.FirstOrDefault();
                PatientVisitHistoryDetailList.PatientVisitHistoryId = model.Id;
                _MedChgContext.PatientVisitHistoryDetailLists.Add(PatientVisitHistoryDetailList);
                await _MedChgContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }




        public async Task<bool> DeletePatientVisitHistory(int visitHistoryId)
        {
            var patientVisitHistory = _MedChgContext.PatientVisitHistoryDetailLists.SingleOrDefault(x => x.Id == visitHistoryId);
            if (patientVisitHistory != null)
            {
                _MedChgContext.PatientVisitHistoryDetailLists.Remove(patientVisitHistory);
                await _MedChgContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public Task<List<Model.PatientVisitHistory>> GetAllVisitHistoryByPatientId(int patientId)
        {
            throw new NotImplementedException();
        }

        public async Task<Model.PatientVisitHistory> AddVisitHistoryFirstTime(Model.PatientVisitHistory model)
        {
            try
            {
                var CheckPatientId = _MedChgContext.PatientVisitHistory.FirstOrDefault(x => x.PatientId == model.PatientId);
                var VisitHistory = new Model.PatientVisitHistory();
                if (CheckPatientId is null)
                {
                    VisitHistory.PatientId = model.PatientId;
                    VisitHistory.PatientAge = model.PatientAge;
                    VisitHistory.PatientAddmissionDate = model.CreatedAt;
                    VisitHistory.PatientAddmissionTime = model.CreatedAt;
                    VisitHistory.CreatedById = model.CreatedById;
                    VisitHistory.Device = "Web";
                    
                    _MedChgContext.Add(VisitHistory);
                    await _MedChgContext.SaveChangesAsync();
                    var PatientVisitHistoryDetailList = new PatientVisitHistoryDetailList();
                    PatientVisitHistoryDetailList = model.PatientVisitHistoryDetailLists.FirstOrDefault();
                    PatientVisitHistoryDetailList.PatientVisitHistoryId = VisitHistory.Id == 0 ? CheckPatientId.Id : VisitHistory.Id;
                    _MedChgContext.PatientVisitHistoryDetailLists.Add(PatientVisitHistoryDetailList);
                    await _MedChgContext.SaveChangesAsync();
                    VisitHistory.PatientVisitHistoryDetailLists = model.PatientVisitHistoryDetailLists;
                }
                return VisitHistory;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<ResponseMessage> UpdateVisitHistory(Model.PatientVisitHistory model)
        {
            try
            {
                var PatientVisitHistory = await _MedChgContext.PatientVisitHistory.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (PatientVisitHistory != null)
                {
                    PatientVisitHistory.UpdatedAt = model.UpdatedAt;
                    PatientVisitHistory.UpdatedById = model.UpdatedById;
                    await _MedChgContext.SaveChangesAsync();
                }
                ResponseMessage.Success = true;
                ResponseMessage.Message = "Succesfully Updated!";
                return ResponseMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Success = false;
                ResponseMessage.Message = ex.Message;
                ResponseMessage.ErrorCode = "UPDATE_ERROR";
                return ResponseMessage;
                throw;
            }
         
        }

        public async Task<List<Model.PatientVisitHistory>> GetAllPatientVisitHistoryByHospitalId(int HospitalId)
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
                            WHERE PVH.""HospitalId""= {HospitalId};
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
                Console.WriteLine(ex.Message);
                throw ex;
            }

        }

        public async Task<PatientVisitHistoryDetailList> AddVisitHistoryDetail(PatientVisitHistoryDetailList model)
        {
            try
            {
                var PatientVisitHistoryDetailList = new PatientVisitHistoryDetailList();
                PatientVisitHistoryDetailList = model;
                _MedChgContext.PatientVisitHistoryDetailLists.Add(PatientVisitHistoryDetailList);
                await _MedChgContext.SaveChangesAsync();
                return PatientVisitHistoryDetailList;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
