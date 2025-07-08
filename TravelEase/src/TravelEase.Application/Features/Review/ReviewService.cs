namespace TravelEase.TravelEase.Application.Features.Review
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<Domain.Entities.Review>> GetReviewsByHotelIdAsync(int hotelId)
        {
            return await _reviewRepository.GetByHotelIdAsync(hotelId);
        }

        public async Task CreateReviewAsync(CreateReviewCommand cmd)
        {
            var review = new Domain.Entities.Review
            {
                HotelId = cmd.HotelId,
                UserId = cmd.UserId,
                Rating = cmd.Rating,
                Comment = cmd.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            await _reviewRepository.DeleteAsync(reviewId);
        }
    }
}