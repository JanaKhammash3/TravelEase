namespace TravelEase.TravelEase.Domain.Entities;

public class Booking
{
    public int Id { get; set; } 
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string SpecialRequests { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int RoomId { get; set; }
    public Room Room { get; set; }
}