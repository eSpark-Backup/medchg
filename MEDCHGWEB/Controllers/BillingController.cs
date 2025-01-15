using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository.BillingRepository;
using MEDCHGClassLibrary.Repository.PatientVisitHistory;
using MEDCHGClassLibrary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MEDCHGWEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly IBillingRepository _BillingRepository;
        private readonly IPatientVisitHistoryRespository _patientVisitHistoryRepository;

        public BillingController(IBillingRepository BillingRepository, IPatientVisitHistoryRespository patientVisitHistoryRepository)
        {
            _BillingRepository = BillingRepository;
            _patientVisitHistoryRepository = patientVisitHistoryRepository;
        }

        // GET: api/<BillingController>
        [HttpGet("GetAllPatientVisitHistory")]
        public async Task<List<PatientVisitHistory>> GetAllPatientVisitHistory()
        {
            var Billing = await _BillingRepository.GetAllPatientVisitHistory();
            return Billing;
        }
        [HttpGet("GetAllPatientVisitHistoryDetailList")]
        public async Task<List<PatientVisitHistoryDetailList>> GetAllPatientVisitHistoryDetailList()
        {
            var Billing = await _BillingRepository.GetAllPatientVisitHistoryDetailList();
            return Billing;
        }

        // GET api/<BillingController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        // GET api/<BillingController>/5
        [HttpGet("GetPatientLocation{id}")]
        public async Task<IActionResult> GetPatientLocation(int Id)
        {
            var Result =  await _BillingRepository.GetPatientLocation(Id);
            if (Result is not null)
            {
                return Ok(Result);
            }
            return BadRequest();
        }
        [HttpGet("GetAllPatientLocation")]
        public async Task<IActionResult> GetAllPatientLocation()
        {
            var Result =  await _BillingRepository.GetAllPatientLocation();
            if (Result is not null)
            {
                return Ok(Result);
            }
            return BadRequest();
        }

        // POST api/<BillingController>
        [HttpPost("UpdateCensusState/{Id}")]
        public async Task<IActionResult> UpdateCensusState(int Id, [FromBody] PatientVisitHistory Model)
        {
            await _BillingRepository.Update(Id, Model);
            return Ok();
        }
        // POST api/<BillingController>
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] PatientVisitHistoryDetailList Model)
        {
            await _BillingRepository.Add(Model);
            return Ok();
        }
        [HttpPost("UpdatePatientInfo")]
        public async Task<IActionResult> UpdatePatientInfo([FromBody] Patient Model)
        {
            var Response = await _BillingRepository.UpdatePatientInfo(Model);
            return Response.Success == true?Ok(Response) :BadRequest(Response);
        }
        [HttpPost("UpdateHistory")]
        public async Task<IActionResult> UpdateHistory([FromBody] PatientVisitHistory Model)
        {
            await _BillingRepository.UpdateHistory(Model);
            return Ok();
        }
        [HttpPost("UpdateVisitHistory")]
        public async Task<IActionResult> UpdateVisitHistory([FromBody] PatientVisitHistory Model)
        {
            await _patientVisitHistoryRepository.UpdateVisitHistory(Model);
            return Ok();
        }

        // PUT api/<BillingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BillingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("GetArchivePatientVisitHistory")]
        public async Task<IActionResult> GetArchivePatientVisitHistory(int providerId, PatientDTO Model)
        {
            var Billing = await _BillingRepository.GetArchivePatientVisitHistory(providerId, Model);
            if (Billing.PatientVisitHistoryDetailLists.Count == 0) return BadRequest(Billing);
            return Ok(Billing);
        }
        [HttpPost("GetBillingReportData")]
        public async Task<List<PatientVisitHistory>> GetBillingReportData(BillingReportModel model)
        {
            var Billing = await _BillingRepository.GetBillingReportData(model);
            return Billing;
        }
    }
}
