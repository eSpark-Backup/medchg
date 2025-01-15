using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.ViewModels.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Repository.PatientRepository
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetAllDischargePatientsByProviderId(int Id);
        Task<string> GetPatientCensusStatusAsync(string MRN, int HospitalId);

		Task<PatientInfoVm> GetPatientInfo(int patientId);
        Task<List<PatientInfoVm>> GetPatientInfoList();
        Task<Model.PatientVisitHistory> UpdatePatientUserStatusAsync(Model.PatientVisitHistory model);
        Task<Patient?> GetPatientById(int patientId);
        Task<Patient?> GetPatientByMRN(string mrn);
        Task<Patient?> GetAdmittedPatientByMRN(string mrn);
        Task AddPatientAsync(PatientDTO model);
        Task<bool> CheckSameMRNAndNameInHospital(PatientDTO patient);
        Task<bool> UpdatePatientCensusStatus(int patientId, int hospitalId, int UpdatedById);
        Task<bool> UpdatePatient(Patient model);
        Task<Patient> GetPatientHistory(int providerId,Patient Model);
        Task<Patient?> GetPatientByMRN(string mrn, int hospitalId);
        Task<bool> UpdateRegisteredPatient(PatientDTO model);
    }
}
