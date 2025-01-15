using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Services;

namespace MEDCHGClassLibrary.Repository.BillingRepository
{
    public interface IBillingRepository
    {
        Task Update(int Id, Model.PatientVisitHistory Model);
        Task UpdateHistory(Model.PatientVisitHistory Model);
        Task<ResponseMessage> UpdatePatientInfo(Patient Model);
        Task Add(PatientVisitHistoryDetailList Model);
        Task<List<Model.PatientVisitHistory>> GetAllPatientVisitHistory();
        Task<List<PatientVisitHistoryDetailList>> GetAllPatientVisitHistoryDetailList();
        Task<PatientDTO> GetArchivePatientVisitHistory(int providerId, PatientDTO Model);
        Task<List<Model.PatientVisitHistory>> GetBillingReportData(BillingReportModel model);
        Task<PatientDTO> GetPatientLocation(int VisitId);
        Task<List<PatientDTO>> GetAllPatientLocation();
    }
}
