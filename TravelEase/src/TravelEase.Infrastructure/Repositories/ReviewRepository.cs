using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly TravelEaseDbContext _context;
    
        public ReviewRepository(TravelEaseDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetByHotelIdAsync(int hotelId)
        {
            return await _context.Reviews.Where(r => r.HotelId == hotelId).ToListAsync();
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }

}