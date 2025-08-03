using TravelEase.Domain.Enums;

namespace TravelEase.Domain.Entities;

public class User
{
    public int Id { get; set; } 
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; } = UserRole.User;

    public ICollection<Booking> Bookings { get; set; }
    public ICollection<Review> Reviews { get; set; }
}