using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository.DiagnosticCodes;
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
    public class DiagnosticCodeController(IDiagnosticCodeRepository _diagnosticCodeRepository,AppState appState) : ControllerBase
    {
        // GET: api/<DiagnosticCodeController>
        [HttpGet("All-DiagnosticCodes")]
        public async Task<ActionResult<IEnumerable<DiagnosticCode>>> Get()
        {
            var diagnosticcodes = await _diagnosticCodeRepository.GetAllDiagnosticCodeAsync();
            return diagnosticcodes;
        }

        // GET api/<DiagnosticCodeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DiagnosticCodeController>
        [HttpPost("Add-DC")]
        public async Task<ActionResult> Post(List<DiagnosticCodeDTO> list)
        {
            try
            {
                List<DiagnosticCode> diagnosticcodes = new();
                List<DiagnosticCode> existingCodes = new();
                foreach (var dto in list)
                {
                    var existingDiagnosticCode = await _diagnosticCodeRepository.GetDiagnosticCodeById(x => x.DiagnosticCodeName == dto.DiagnosticCodeName && x.ProviderId == dto.ProviderId);
                    if(existingDiagnosticCode == null)
                    {
                        var entity = new DiagnosticCode
                        {
                            DiagnosticCodeName = dto.DiagnosticCodeName,
                            DiagnosticCodeDescription = dto.DiagnosticCodeDescription,
                            ProviderId = dto.ProviderId.Value,
                            CreatedById = dto.CreatedById,
                            Device = "WEB"
                            // Map other properties as needed
                        };
                        diagnosticcodes.Add(entity);
                    }
                    else
                    {
                        existingCodes.Add(existingDiagnosticCode);
                    }
                }
                await _diagnosticCodeRepository.AddDiagnosticCodeAsync(diagnosticcodes);
                return Ok(existingCodes);

            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException.Message.Contains("IX_DiagnosticCodes_DiagnosticCodeName_ProviderId"))
                {
                    return BadRequest($"Diagnostic Code Name already exists.");
                }

                return BadRequest(ex.InnerException.Message);
            }
        }

        // PUT api/<DiagnosticCodeController>/5
        [HttpPut("UpdateByFav{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] bool fav)
        {

            await _diagnosticCodeRepository.UpdateDiagnosticCodeByIdAsync(id, fav,Convert.ToInt32(appState.currentUser.Id));
            return Ok();
        }

        // DELETE api/<DiagnosticCodeController>/5
        [HttpDelete("Delete{id}")]
        public async Task<ActionResult<bool>> Delete(int Id)
        {
            var result = await _diagnosticCodeRepository.DeleteDiagnosticCodeAsync(Id, appState.currentUser.Id);
            return Ok(result);
        }
    }
}
