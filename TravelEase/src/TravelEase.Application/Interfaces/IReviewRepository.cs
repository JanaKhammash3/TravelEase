using TravelEase.TravelEase.Domain.Entities;

public interface IReviewRepository
{
    Task<List<Review>> GetByHotelIdAsync(int hotelId);
    Task AddAsync(Review review);
    Task DeleteAsync(int reviewId);
}