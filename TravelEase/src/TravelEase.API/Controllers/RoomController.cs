using Microsoft.AspNetCore.Mvc;
using TravelEase.TravelEase.Application.Features.Room;

namespace TravelEase.TravelEase.API.Controllers
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
        
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRoomsQuery query)
        {
            var results = await _roomService.SearchRoomsAsync(query);
            return Ok(results);
        }


    }
}