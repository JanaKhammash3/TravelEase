using Microsoft.AspNetCore.Mvc;
using TravelEase.TravelEase.Application.Features.Room;
using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoomCommand cmd)
        {
            await _roomService.CreateRoomAsync(cmd);
            return Ok(new { message = "Room created successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateRoomCommand cmd)
        {
            await _roomService.UpdateRoomAsync(id, cmd);
            return Ok(new { message = "Room updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _roomService.DeleteRoomAsync(id);
            return Ok(new { message = "Room deleted successfully" });
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomCommand command)
        {
            command.Id = id;
            await _roomService.UpdateRoomAsync(command);
            return Ok(new { message = "Room updated successfully" });
        }

    }
}