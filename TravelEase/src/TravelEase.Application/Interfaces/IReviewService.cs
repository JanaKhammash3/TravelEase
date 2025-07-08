using TravelEase.TravelEase.Application.DTOs;

public interface IReviewService
{
    Task CreateReviewAsync(ReviewDto dto);
    Task<List<ReviewResponseDto>> GetReviewsByHotelIdAsync(int hotelId);
}