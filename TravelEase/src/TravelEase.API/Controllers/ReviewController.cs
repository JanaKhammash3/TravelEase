using Microsoft.AspNetCore.Mvc;
using TravelEase.TravelEase.Application.Features.Review;

namespace TravelEase.TravelEase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetReviewsForHotel(int hotelId)
        {
            var reviews = await _reviewService.GetReviewsByHotelIdAsync(hotelId);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand cmd)
        {
            await _reviewService.CreateReviewAsync(cmd);
            return Ok(new { message = "Review added successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return Ok(new { message = "Review deleted successfully." });
        }
    }
}