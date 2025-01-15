using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository.ChargeCodes;
using MEDCHGWEB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MEDCHGWEB.Controllers
{
    [Authorize(Policy = "ApiPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeCodeController : ControllerBase
    {
        private readonly IChargeCodeRepository _chargeCodeRepository;
        private readonly AppState appState;

        public ChargeCodeController(IChargeCodeRepository chargeCodeRepository, AppState appState)
        {
            _chargeCodeRepository = chargeCodeRepository;
            this.appState = appState;
        }

        // GET: api/<ChargeCodeController>
        [HttpGet("All-ChargeCodes")]
        public async Task<ActionResult<IEnumerable<ChargeCode>>> Get()
        {
            var chargecodes = await _chargeCodeRepository.GetAllChargeCodeAsync();
            return chargecodes;
        }

        // GET api/<ChargeCodeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value ";
        }

        // POST api/<ChargeCodeController>
        [HttpPost("Add-CC")]
        public async Task<ActionResult> Post(List<ChargeCodeDTO> list)
        {
            try
            {
                List<ChargeCode> chargecodes = new List<ChargeCode>();
                List<ChargeCode> exisitingChargecodes = new List<ChargeCode>();
                foreach (var dto in list)
                {
                    var existingChargeCode = await _chargeCodeRepository.GetChargeCodeById(x => x.ChargeCodeName == dto.ChargeCodeName && x.ProviderId == dto.ProviderId);
                    if(existingChargeCode == null)
                    {
                        var entity = new ChargeCode
                        {
                            ChargeCodeName = dto.ChargeCodeName,
                            ChargeCodeDescription = dto.ChargeCodeDescription,
                            ProviderId = dto.ProviderId.Value,
                            CreatedById = dto.CreatedById,
                            Device = "WEB"
                            // Map other properties as needed
                        };
                        chargecodes.Add(entity);
                    }
                    else
                    {
                        exisitingChargecodes.Add(existingChargeCode);
                    }
                }
                await _chargeCodeRepository.AddChargeCodeAsync(chargecodes);
                return Ok();
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException.Message.Contains("IX_ChargeCodes_ChargeCodeName_ProviderId"))
                {
                    return BadRequest($"Charge Code Name already exists.");
                }

                return BadRequest(ex.InnerException.Message);
            }
        }

        // PUT api/<ChargeCodeController>/5
        [HttpPut("UpdateByFav{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] bool fav, int UpdatedById)
        {
            await _chargeCodeRepository.UpdateChargeCodeByIdAsync(id, fav,Convert.ToInt32(appState.currentUser.Id));
            return Ok();
        }

        // DELETE api/<ChargeCodeController>/5
        [HttpDelete("Delete{id}")]
        public async Task<ActionResult<bool>> Delete(int Id)
        {
            var result = await _chargeCodeRepository.DeleteChargeCodeAsync(Id,appState.currentUser.Id);
            return Ok(result);
        }
    }
}
