using TravelEase.Domain.Entities;

namespace TravelEase.TravelEase.Application.Interfaces;

public interface IReviewRepository
{
    Task<List<Review>> GetByHotelIdAsync(int hotelId);
    Task AddAsync(Review review);
    Task DeleteAsync(int reviewId);
}