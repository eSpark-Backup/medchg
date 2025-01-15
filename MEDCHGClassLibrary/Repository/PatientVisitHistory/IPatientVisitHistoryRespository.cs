using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Repository.PatientVisitHistory
{
    public interface IPatientVisitHistoryRespository
    {
        Task AddVisitHistory(Model.PatientVisitHistory model);
        Task<PatientVisitHistoryDetailList> AddVisitHistoryDetail(PatientVisitHistoryDetailList model);
        Task<ResponseMessage> UpdateVisitHistory(Model.PatientVisitHistory model);
        Task<Model.PatientVisitHistory> AddVisitHistoryFirstTime(Model.PatientVisitHistory model);
        Task<List<Model.PatientVisitHistory>> GetAllVisitHistoryByPatientId(int patientId);
        Task<List<Model.PatientVisitHistory>> GetAllPatientVisitHistoryByHospitalId(int HospitalId);
        Task<bool> DeletePatientVisitHistory(int visitHistoryId);
    }
}
