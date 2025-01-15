using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository;
using MEDCHGClassLibrary.Repository.BuildingRepository;
using MEDCHGClassLibrary.ViewModels.Building;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MEDCHGWEB.Controllers
{
    [Authorize(Policy = "ApiPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : Controller
    {
        private readonly IRepository<Building> _buildingRepository;
        private readonly IBuildingRepository _buildingsRepository;
        public BuildingController(IRepository<Building> buildingRepository, IBuildingRepository buildingsRepository)
        {
            _buildingRepository = buildingRepository;
            _buildingsRepository = buildingsRepository;
        }

        [HttpPost("AddBuilding")]
        public async Task<IActionResult> AddBuilding(Building model)
        {
            var Building = await _buildingsRepository.AddBuilding(model);
            if (Building.Success == true) return Ok(Building);
            return BadRequest(Building);
        }

        [HttpGet("GetBuildingById/{id}")]
        public async Task<IActionResult> GetBuildingById([FromRoute] int id)
        {
            var response = await _buildingsRepository.GetBuildingById(id);
            if (response == null) return BadRequest();
            return Ok(response);
        }

        [HttpPut("UpdateBuilding")]
        public async Task<IActionResult> UpdateBuilding([FromBody] Building model)
        {
           var response =  await _buildingsRepository.UpdateBuilding(model);
            if (response.Success == false) return BadRequest();
            return Ok(model);
        }

        [HttpGet("BuildingPatients")]
        public async Task<ActionResult<BuildingPatientListVm>> GetBuildingPatients(int id)
        {
            try
            {
                var result = await _buildingsRepository.GetBuildingPatients(id);
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

        [HttpGet("GetAllHospitalsWithPatients")]
        public async Task<string> GetDBuilding(int ID)

        {
            var b = _buildingRepository.GetAsync(x => x.Id == ID, null, null, "Floors,Rooms");

            var building = JsonConvert.SerializeObject(b);

            return building;
        }
        [HttpGet("GetAllBuildings")]
        public async Task<ActionResult<Building>> GetAllBuildings()
        {
            var buildings = await _buildingsRepository.GetAllBuildings();
            return Ok(buildings);
        }

        [HttpPost("DeleteBuilding/{id}")]

        public async Task<ActionResult<bool>> DeleteBuilding([FromRoute] int id)
        {
            var result = await _buildingsRepository.DeleteBuilding(id);
            return Ok(result);
        }
    }
}
