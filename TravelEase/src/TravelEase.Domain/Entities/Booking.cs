using TravelEase.TravelEase.Domain.Enums;

namespace TravelEase.TravelEase.Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }

    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }

    public int Adults { get; set; }
    public int Children { get; set; }
    public string SpecialRequests { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }
    public string PaymentStatus { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Card;

    public User User { get; set; }
    public Room Room { get; set; }
}