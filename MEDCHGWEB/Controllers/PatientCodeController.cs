using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository;
using MEDCHGClassLibrary.Repository.PatientRepository;
using Microsoft.AspNetCore.Authorization;
using MEDCHGClassLibrary.Repository.PatientVisitHistory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MEDCHGWEB.Controllers
{
    [Authorize(Policy = "ApiPolicy")]
    [Route("api/PatientCode/DiagnosticCode")]
    [Route("api/PatientCode/ChargeCode")]
    [Route("api/PatientCode/TempCode")]
    [Route("api/PatientCode/VisitHistory")]
    [Route("api/PatientCode/User")]
    public class PatientCodeController : Controller
    {

        private readonly IRepository<PatientTempCode> _PatientTempCodeRepository;
        private readonly IPatientRepository _PatientsRepository;
        private readonly IPatientVisitHistoryRespository _patientVisitHistoryRepository;
        public PatientCodeController(IRepository<PatientTempCode> patientTempCodeRepository,
                                        IPatientRepository patientsRepository,
                                        IPatientVisitHistoryRespository patientVisitHistoryRepository)
        {
            _PatientTempCodeRepository = patientTempCodeRepository;
            _PatientsRepository = patientsRepository;
            _patientVisitHistoryRepository = patientVisitHistoryRepository;
        }

        [HttpPost("AddVisitHistory")]
        public async Task<IActionResult> AddVisitHistory([FromBody] PatientVisitHistory model)
        {
            try
            {
                await _patientVisitHistoryRepository.AddVisitHistory(model);
                return Ok();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("IX_Patients_PatientName"))
                {
                    return BadRequest("Patient Name already exist");
                }

                return BadRequest(ex.InnerException.Message);
            }
        }
        [HttpPost("AddVisitHistoryDetail")]
        public async Task<ActionResult<PatientVisitHistory>> AddVisitHistoryDetail([FromBody] PatientVisitHistoryDetailList model)
        {
            try
            {
                var result = await _patientVisitHistoryRepository.AddVisitHistoryDetail(model);
                if (result == null) return BadRequest();
                return Ok(result);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("IX_Patients_PatientName"))
                {
                    return BadRequest("Patient Name already exist");
                }

                return BadRequest(ex.InnerException.Message);
            }
        }
        [HttpPost("AddVisitHistoryFirstTime")]
        public async Task<ActionResult<PatientVisitHistory>> AddVisitHistoryFirstTime([FromBody] PatientVisitHistory model)
        {
            try
            {
                var result = await _patientVisitHistoryRepository.AddVisitHistoryFirstTime(model);
                if (result == null) return BadRequest();
                return Ok(result);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("IX_Patients_PatientName"))
                {
                    return BadRequest("Patient Name already exist");
                }

                return BadRequest(ex.InnerException.Message);
            }
        }

        [HttpDelete("DeleteVisitHistory/{id}")]
        public async Task<ActionResult<bool>> DeleteVisitHistory(int id)
        {
            var result = await _patientVisitHistoryRepository.DeletePatientVisitHistory(id);
            if (result == false) return BadRequest();
            
            return Ok(result);
        }
        [HttpPost("StatusChange")]
        public async  Task<ActionResult> UpdatePatientUserStatusAsync([FromBody] PatientVisitHistory model)
        {
            var patients = await _PatientsRepository.UpdatePatientUserStatusAsync(model);
            return Ok(patients);
        }

        [HttpDelete("DeleteTempCode/{id}")]
        public async Task<IActionResult> DeleteTempCode(int id)

        {
            var result = await _PatientTempCodeRepository.DeleteAsync(id);
            return Ok(result);
        }

    }
}
