using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelEase.TravelEase.Application.Features.Hotel;
using TravelEase.TravelEase.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using TravelEase.TravelEase.API.Models;
using TravelEase.TravelEase.Infrastructure.Services;

namespace TravelEase.TravelEase.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IHotelRepository _hotelRepository;

        public HotelController(IHotelService hotelService, IHotelRepository hotelRepository)
        {
            _hotelService = hotelService;
            _hotelRepository = hotelRepository;
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchHotels([FromQuery] SearchHotelsQuery query)

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

        // ✅ Record view when user visits hotel page
        [HttpPost("{hotelId}/view")]
        public async Task<IActionResult> RecordHotelView(int hotelId)
        {
            int userId = GetUserIdFromToken();
            await _hotelRepository.RecordHotelViewAsync(userId, hotelId);
            return Ok();
        }

        // ✅ Get top 5 most visited cities
        [HttpGet("trending-destinations")]
        public async Task<IActionResult> GetTrendingDestinations()
        {
            var topCities = await _hotelRepository.GetTrendingCitiesAsync();
            return Ok(topCities);
        }

        // ✅ Helper to extract user ID from JWT token
        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token.");

            return int.Parse(userIdClaim.Value);
        }
        
        [HttpGet("recent-hotels")]
        public async Task<IActionResult> GetRecentHotels([FromQuery] int count = 5)
        {
            int userId = GetUserIdFromToken();
            var hotels = await _hotelRepository.GetRecentlyVisitedHotelsAsync(userId, count);
            return Ok(hotels);
        }
        [HttpPost("{id}/upload-images")]
        public async Task<IActionResult> UploadImages(int id, [FromForm] HotelImageUploadDto dto, [FromServices] CloudinaryImageService cloudinaryService)
        {
            var urls = new List<string>();

            foreach (var file in dto.Images)
            {
                var url = await cloudinaryService.UploadImageAsync(file);
                urls.Add(url);
            }

            await _hotelService.SaveHotelImageUrlsAsync(id, urls); // <-- Save to DB

            return Ok(new { UploadedUrls = urls });
        }

        

        [HttpGet("{id}/nearby")]
        public async Task<IActionResult> GetNearbyAttractions(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
                return NotFound("Hotel not found.");

            // Mocked static attractions
            var attractions = new List<object>
            {
                new { Name = "City Museum", Type = "Museum", DistanceKm = 1.5 },
                new { Name = "Central Park", Type = "Park", DistanceKm = 0.8 },
                new { Name = "Grand Theater", Type = "Entertainment", DistanceKm = 2.1 }
            };

            return Ok(new
            {
                Coordinates = new { hotel.Latitude, hotel.Longitude },
                NearbyAttractions = attractions
            });
        }




    }
}
