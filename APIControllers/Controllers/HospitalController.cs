using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository.Hospital;
using MEDCHGClassLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using MEDCHGClassLibrary.ViewModels.Hospital;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIControllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController(IRepository<Hospital> hospitalRepository, IHospitalRepository _hospitalsRepository) : ControllerBase
    {
        // GET: api/<HospitalController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<HospitalController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HospitalController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
        [HttpGet("HospitalWithProvider")]
        public async Task<IActionResult> GetHospitalsWithProvider([FromQuery] int pid, int uid)
        {
            try
            {
                var result = await _hospitalsRepository.GetHospitalWithProvider(pid, uid);
                if (result == null)
                {
                    return BadRequest();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        // PUT api/<HospitalController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HospitalController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
