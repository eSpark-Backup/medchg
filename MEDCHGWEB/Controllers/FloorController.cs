using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository;
using MEDCHGClassLibrary.Repository.FloorRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MEDCHGWEB.Controllers
{
    [Authorize(Policy = "ApiPolicy")]
    [Route("api/hospital/building/{buildingId}/floor")]
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : Controller
    {
        private readonly IRepository<Floor> _FloorRepository;
        private readonly IFloorRepository _floorsRepository;
        public FloorController(IRepository<Floor> floorRepository, IFloorRepository floorsRepository)
        {
            _FloorRepository = floorRepository;
            _floorsRepository = floorsRepository;
        }

        [HttpPost("AddFloor")]
        public async Task<ActionResult<Floor>> AddFloors(Floor model)
        {
            var Floor = await _floorsRepository.AddFloor(model);
            if (Floor.Success == true) return Ok(Floor);
            return BadRequest(Floor);
        }
        [HttpPost("UpdateFloor")]
        public async Task<IActionResult> UpdateFloors(Floor model)
        {
            var response = await _floorsRepository.UpdateFloor(model);
            if (response.Success == true)
            {
                return Ok(model);
            }
            return BadRequest(response);
        }
        [HttpDelete("DeleteFloor/{Id}/{UpdatedById}")]
        public async Task<IActionResult> DeleteFloors(int Id, int UpdatedById)
        {
            var resp = await _floorsRepository.DeleteFloor(Id, UpdatedById);
            if (!resp.Success)
            {
                return BadRequest(resp);
            }
            return Ok(resp);
        }

        [HttpGet]

        public async Task<ActionResult<string>> GetFloor(int buildingId)
        {
            try
            {
                var resp = await _FloorRepository.GetAsync(x => x.BuildingId == buildingId, null, null, "");

                //var selector = resp.Select(x => new { x.id, x.id, PatCount = x.Patients?.Count, Status = "N/A" });

                return Ok(JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }
        [HttpGet("GetAllFloors")]
        public async Task<ActionResult<Floor>> GetAllFloors()
        {
            var floors = await _floorsRepository.GetAllFloors();
            return Ok(floors);
        }
    }
}
