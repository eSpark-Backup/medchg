using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository;
using MEDCHGClassLibrary.Repository.ProviderRepository;
using MEDCHGWEB.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MEDCHGWEB.Controllers
{
    [Authorize(Policy = "ApiPolicy")]
    [Route("api/Provider/{ProviderId}")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderRepository _providerRepository;
        public ProviderController(IProviderRepository providerRepository, IRepository<Provider> providerGenericRepository)
        {
            _providerRepository = providerRepository;
        }
        // GET: api/<ProviderController>
        [HttpGet("GetAllProviders")]
        public async Task<ActionResult<Provider>> Get()
        {
            var provider = await _providerRepository.GetAllProviderAsync();
            return Ok(provider);
        }
        // GET api/<ProviderController>/5
        [HttpGet("Get-Provider{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var provider = await _providerRepository.GetProviderByIdAsync(id);
            return Ok(provider);
        }
        [HttpGet("GetAllHospitalsByProviderId{Id}")]
        public async Task<ActionResult<Hospital>> GetAllHospitalsByProviderId(int Id)
        {
            var hospitals = await _providerRepository.GetAllHospitalsByProviderId(Id);
            return Ok(hospitals);
        }


        [HttpGet("GetAllDiagnosticCodesByProviderId{Id}")]
        public async Task<ActionResult<DiagnosticCode>> GetAllDiagnosticCodesByProviderId(int Id)
        {
            var diagnosticCodes = await _providerRepository.GetAllDiagnosticCodesByProviderId(Id);
            return Ok(diagnosticCodes);
        }

        [HttpGet("GetAllChargeCodesByProviderId{Id}")]
        public async Task<ActionResult<ChargeCode>> GetAllChargeCodesByProviderId(int Id)
        {
            var chargeCodes = await _providerRepository.GetAllChargeCodesByProviderId(Id);
            return Ok(chargeCodes);
        }

        [HttpGet("GetAllUsersByProviderId{Id}")]
        public async Task<ActionResult<ChargeCode>> GetAllUsersByProviderId(int Id)
        {
            var chargeCodes = await _providerRepository.GetAllUsersByProviderId(Id);
            return Ok(chargeCodes);
        }
        // POST api/<ProviderController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
        [HttpPost("Add-Provider")]
        public async Task<ActionResult<Provider>> Post(ProviderDTO model)
        {
            var entity = new Provider
            {
                Name = model.Name.Replace(" ", "").ToLower(),
                DisplayName = model.DisplayName,
                Device = "WEB",
                CreatedById = model.CreatedAndUpdatedById,
                // Map other properties as needed
            };
            var response = await _providerRepository.AddProviderAsync(entity);
            return Ok(response);
        }
        // PUT api/<ProviderController>/5
        [HttpPut("StatusChange")]
        public async Task<ActionResult> Put([FromBody] ProviderDTO model)
        {
            var providerModel = new Provider()
            {
                Id = model.Id.Value,
                Name = model.Name,
                DisplayName = model.DisplayName,
                IsActive = model.IsActive.Value,
                UpdatedById = model.CreatedAndUpdatedById,
            };
            var provider = await _providerRepository.UpdateProviderStatusAsync(providerModel);
            return Ok(provider);
        }
        // PUT api/<ProviderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProviderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [HttpGet]
        public async Task<IActionResult> GetByProviderId([FromRoute] int ProviderId)
        {
            var resp = await _providerRepository.GetProviderUsersAsync(ProviderId);
            if(resp == null) return BadRequest(resp);
            return Ok(resp);
        }

        [HttpGet("GetTimerSettings{Id}")]
        public async Task<ActionResult<ChargeCode>> GetTimerSettings(int Id)
        {
            var timerSettings = await _providerRepository.GetTimerSettings(Id);
            return Ok(timerSettings);
        }

        [HttpPost("SaveTimerSettings")]
        public async Task<ActionResult<ChargeCode>> SaveTimerSettings(ProviderTimerSetting model)
        {
            var timerSettings = await _providerRepository.SaveTimerSettings(model);
            return Ok(timerSettings);
        }
    }
}
