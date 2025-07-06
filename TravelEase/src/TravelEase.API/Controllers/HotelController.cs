using Microsoft.AspNetCore.Mvc;
using TravelEase.TravelEase.Application.Features.Hotel;

namespace TravelEase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly HotelService _hotelService;

        public HotelController(HotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _hotelService.GetAllHotelsAsync();
            return Ok(hotels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelById(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelCommand cmd)
        {
            await _hotelService.CreateHotelAsync(cmd);
            return Ok(new { message = "Hotel created successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelCommand cmd)
        {
            cmd.Id = id;
            await _hotelService.UpdateHotelAsync(cmd);
            return Ok(new { message = "Hotel updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            await _hotelService.DeleteHotelAsync(id);
            return Ok(new { message = "Hotel deleted successfully." });
        }
    }
}