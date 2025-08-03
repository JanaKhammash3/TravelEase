using TravelEase.Application.DTOs;
using TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.Application.Features.Review
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;

        public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
        }

        public async Task CreateReviewAsync(ReviewDto dto)
        {
            var review = new Domain.Entities.Review
            {
                HotelId = dto.HotelId,
                UserId = dto.UserId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
        }

        public async Task<List<ReviewResponseDto>> GetReviewsByHotelIdAsync(int hotelId)
        {
            var reviews = await _reviewRepository.GetByHotelIdAsync(hotelId);

            return reviews.Select(r => new ReviewResponseDto
            {
                UserName = r.User?.FullName ?? "Unknown",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            await _reviewRepository.DeleteAsync(reviewId);
        }
    }
}