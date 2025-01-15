using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository;
using MEDCHGClassLibrary.Repository.Hospital;
using MEDCHGClassLibrary.ViewModels.Hospital;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MEDCHGWEB.Controllers
{
    [Authorize(Policy = "ApiPolicy")]
    [Route("api/hospital/{hospitalId}")]
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController : Controller
    {

        private readonly IRepository<Hospital> _HospitalRepository;
        private readonly IHospitalRepository _hospitalsRepository;
        public HospitalController(IRepository<Hospital> hospitalRepository, IHospitalRepository hospitalsRepository)
        {
            _HospitalRepository = hospitalRepository;
            _hospitalsRepository = hospitalsRepository;
        }

        // PUT api/<HospitalController>/5
        [HttpPut("UpdateByFav{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] bool fav)
        {
            await _HospitalRepository.UpdateHospitalByIdAsync(id, fav);
            return Ok();
        }

        [HttpPost("AddHospital")]

        public async Task<ActionResult<Hospital>> AddHospital(Hospital model)
        {
            try
            {
                var exisitingHospital = await _hospitalsRepository.CheckHospitalNameUnique(model);
                if (exisitingHospital.Success == true)
                {
                    var response = await _hospitalsRepository.AddHospital(model);
                    if (response.Success == true) return Ok(response);
                    return BadRequest(response);
                }
                return BadRequest("Hospital already exist");
            }

            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("IX_Hospitals_HospitalName_ProvidersId"))
                {
                    return BadRequest("Hospital Name already exist");
                }

                return BadRequest(ex.InnerException.Message);

            }
        }

        [HttpGet("GetAllHospitals")]
        public async Task<ActionResult<Hospital>> GetAllHospitals()
        {
            var hospitals = await _hospitalsRepository.GetAllHospitals();
            return Ok(hospitals);
        }

        [HttpGet("HospitalWithProvider")]
        public async Task<ActionResult<HospitalListVm>> GetHospitalsWithProvider([FromQuery] int pid, int uid)
        {
            try
            {
                var result = await _hospitalsRepository.GetHospitalWithProvider(pid, uid);
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
        [HttpGet("HospitalBuilding")]
        public async Task<ActionResult<HospitalBuildingListVm>> GetHospitalBuildings(int hospId)
        {
            try
            {
                var result = await _hospitalsRepository.GetHospitalBuildings(hospId);
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

        [HttpGet("GetHospitalById")]
        public async Task<IActionResult> GetHospitalById([FromQuery] int hospId)
        {
            var response = await _hospitalsRepository.GetHospitalById(hospId);
            if (response == null)
            {
                return BadRequest();
            }
            return Ok(response);
        }


        [HttpGet]

        public async Task<ActionResult<string>> Get(int hospitalId)
        {
            var resp = await _HospitalRepository.GetAsync(
    Singlefilter: x => x.Id == hospitalId,
    orderBy: null,
    includeProperties: "Buildings.Floors.Rooms"
);
            return JsonConvert.SerializeObject(resp.First(), Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

        }

        [HttpGet("GetBuildingFloorsRoomsLists/{Id}")]
        public async Task<IActionResult> GetIt(int Id)
        {
            var hospitals = await _hospitalsRepository.GetHospitalBuildingsRooms(Id);
            if (hospitals == null) return BadRequest(hospitals);
            return Ok(hospitals);
        }

        [HttpPost("TestApi")]
        public IActionResult Test() { return Ok(); }

        [HttpPost("DeleteHospital/{id}")]

        public async Task<ActionResult<bool>> DeleteHospital([FromRoute] int id)
        {
            var result = await _hospitalsRepository.DeleteHospital(id);
            return Ok(result);
        }

    }
}
