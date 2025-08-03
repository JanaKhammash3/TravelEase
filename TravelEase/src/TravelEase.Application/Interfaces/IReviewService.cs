using TravelEase.Application.DTOs;

namespace TravelEase.TravelEase.Application.Interfaces;
public interface IReviewService
{
    Task CreateReviewAsync(ReviewDto dto);
    Task<List<ReviewResponseDto>> GetReviewsByHotelIdAsync(int hotelId);
}