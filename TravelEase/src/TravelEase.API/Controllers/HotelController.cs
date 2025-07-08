using Microsoft.AspNetCore.Mvc;
using TravelEase.Application.Features.Hotel;
using TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Application.Features.Hotel;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
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
            return hotel == null ? NotFound() : Ok(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelCommand cmd)
        {
            await _hotelService.CreateHotelAsync(cmd);
            return Ok(new { message = "Hotel created successfully." });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHotel([FromBody] UpdateHotelCommand cmd)
        {
            await _hotelService.UpdateHotelAsync(cmd);
            return Ok(new { message = "Hotel updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            await _hotelService.DeleteHotelAsync(id);
            return Ok(new { message = "Hotel deleted successfully." });
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchHotels([FromBody] SearchHotelsQuery query)
        {
            var result = await _hotelService.SearchHotelsAsync(query);
            return Ok(result);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedHotels()
        {
            var result = await _hotelService.GetFeaturedHotelsAsync();
            return Ok(result);
        }
    }
}
