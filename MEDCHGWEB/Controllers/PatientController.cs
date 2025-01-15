using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository;
using MEDCHGClassLibrary.Repository.PatientRepository;
using MEDCHGClassLibrary.Repository.PatientVisitHistory;
using MEDCHGClassLibrary.ViewModels.Patient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace MEDCHGWEB.Controllers
{
    [Authorize(Policy = "ApiPolicy")]
    [Route("api/Patient/{identifier}")]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : Controller
    {
        private readonly IRepository<Patient> _PatientRepository;
        private readonly IPatientVisitHistoryRespository _PatientVisitHistoryRepository;
        private readonly IPatientRepository _patientRepository;
        public PatientController(IRepository<Patient> PatientRepository, IPatientRepository patientRepository, IPatientVisitHistoryRespository patientVisitHistoryRepository)
        {
            _PatientRepository = PatientRepository;
            _patientRepository = patientRepository;
            _PatientVisitHistoryRepository = patientVisitHistoryRepository;
        }

        [HttpPost("AddPatient")]
        public async Task<ActionResult<Patient>> AddPatient(PatientDTO DTO)
        {
            try
            {
                var exisitingPatient = await _patientRepository.CheckSameMRNAndNameInHospital(DTO);
                if (exisitingPatient == true)
                {
                    await _patientRepository.AddPatientAsync(DTO);
                    return Ok(DTO);
                }
                return BadRequest();
            }

            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("IX_Patients_PatientName"))
                {
                    return BadRequest("Patient Name already exist");
                }
                return BadRequest(ex.InnerException.Message);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UpdatePatientInfo")]

        public async Task<ActionResult> UpdatePatientInfo(PatientDTO model)
        {
            try
            {
                var result = await _patientRepository.UpdateRegisteredPatient(model);
                if (result == false) return BadRequest();
                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }
        [HttpGet("GetAdmittedPatientByMRN/{MRN}")]
        public async Task<ActionResult<Patient>> GetAdmittedPatientByMRN(string MRN)
        {
            var result = await _patientRepository.GetAdmittedPatientByMRN(MRN);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("GetPatientByMRN/{MRN}")]
        public async Task<ActionResult<Patient>> GetPatientByMRN(string MRN)
        {
            var result = await _patientRepository.GetPatientByMRN(MRN);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("GetPatientByMRN&Hospital")]
        public async Task<ActionResult<PatientDTO>> GetPatientByMRN([FromQuery] string mrn, int hospId)
        {
            var result = await _patientRepository.GetPatientByMRN(mrn, hospId);
            if (result == null)
            {
                var AAD = new PatientDTO();
                return Ok(AAD);
            }
            var model = new PatientDTO();
            model.PateintId = result.Id;
            model.MRNNumber = result.MRN;
            model.FirstName = result.FirstName;
            model.MiddleName = result.MiddleName;
            model.LastName = result.LastName;
            model.Gender = result.Gender;
            model.Salutation = result.Salutation;
            model.DOB = result.DateOfBirth;
            model.HospitalId = result.PatientVisitHistories.PatientVisitHistoryDetailLists.FirstOrDefault().HospitalId;
            model.UserId = result.PatientVisitHistories.UserId;
            return Ok(model);
        }

        [HttpGet("PatientInformation")]

        public async Task<ActionResult<PatientInfoVm>> GetPatient([FromQuery] int pId)
        {
            try
            {
                var result = await _patientRepository.GetPatientInfo(pId);
                if (result == null)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("PatientInformationList")]
        public async Task<ActionResult<List<PatientInfoVm>>> GetPatientInfoList()
        {
            try
            {
                var result = await _patientRepository.GetPatientInfoList();
                if (result == null)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("UpdatePatientCensus")]
        public async Task<ActionResult<bool>> UpdatePatientCensus([FromQuery] int pId, int hospId, int UpdatedById)
        {
            var result = await _patientRepository.UpdatePatientCensusStatus(pId, hospId, UpdatedById);
            if (!result) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetPatientCensusStatus/{MRN}/{HospitalId}")]
        public async Task<IActionResult> PatientCensusStatus([FromRoute] string MRN, [FromRoute] int HospitalId)
        {
            var Response = await _patientRepository.GetPatientCensusStatusAsync(MRN, HospitalId);
            if (Response == "0") return Ok(Response);
            return BadRequest(Response);
        }

        [HttpGet("GetAllPatientVisitHistoryByHospitalId/{HospitalId}")]
        public async Task<ActionResult<List<PatientVisitHistory>>> GetAllPatientVisitHistoryByHospitalId(int HospitalId)
        {
            var result = await _PatientVisitHistoryRepository.GetAllPatientVisitHistoryByHospitalId(HospitalId);
            if (result.Count != 0) return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("GetPatientById/{id}")]
        public async Task<ActionResult<Patient>> GetPatientInformation([FromRoute] int id)
        {
            var result = await _patientRepository.GetPatientById(id);
            if (result == null) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("UpdatePatientInformation")]
        public async Task<ActionResult<bool>> UpdatePatientInformation(Patient model)
        {
            var result = await _patientRepository.UpdatePatient(model);
            if (!result) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("PatientVisitHistory")]
        public async Task<ActionResult<Patient>> GetPatientHistory(int providerId,Patient Model)
        {
            try
            {
                var result = await _patientRepository.GetPatientHistory(providerId, Model);
                if (result.PatientVisitHistories.Id == 0) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception e)
            {
                throw;
            }

        }

        [HttpGet("GetAllDischargePatientsByProviderId{Id}")]
        public async Task<ActionResult<List<Patient>>> GetAllDischargePatientsByProviderId(int Id)
        {
            try
            {
                var result = await _patientRepository.GetAllDischargePatientsByProviderId(Id);
                if (result == null) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}
