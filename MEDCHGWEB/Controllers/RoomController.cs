using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository;
using MEDCHGClassLibrary.Repository.RoomRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MEDCHGWEB.Controllers
{
    [Authorize(Policy = "ApiPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : Controller
    {
        private readonly IRepository<Room> _RoomRepository;
        private readonly IRoomRepository _roomsRepository;
        public RoomController(IRepository<Room> RoomRepository, IRoomRepository roomsRepository)
        {
            _RoomRepository = RoomRepository;
            _roomsRepository = roomsRepository;
        }
        [HttpGet("GetAllRooms")]
        public async Task<ActionResult<Room>> GetAllRooms()
        {
            var rooms = await _roomsRepository.GetAllRooms();
            return Ok(rooms);
        }
        [HttpGet("GetAllOccupiedRooms/{HospitalId}")]
        public async Task<ActionResult<string>> GetAllOccupiedRooms(int HospitalId)
        {
            var rooms = await _roomsRepository.GetAllOccupiedRooms(HospitalId);
            return Ok(rooms);
        }

        [HttpPost("UpdateRoom")]
        public async Task<IActionResult> UpdateRoom(Room model)
        {
            await _RoomRepository.UpdateAsync(model);
            return Ok(model);
        }

        [HttpDelete("DeleteRoom/{id}/{UpdatedById}")]
        public async Task<IActionResult> DeleteRoom(int id,int UpdatedById)
        {
            var result = await _roomsRepository.DeleteRoom(id, UpdatedById);
            if (result is false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
