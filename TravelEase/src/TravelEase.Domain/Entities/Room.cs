using TravelEase.TravelEase.Domain.Enums;

namespace TravelEase.TravelEase.Domain.Entities;

public class Room
{
    public int Id { get; set; } 
    public string Number { get; set; }
    public int CapacityAdults { get; set; }
    public int CapacityChildren { get; set; }
    public decimal PricePerNight { get; set; }
    public RoomCategory Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int HotelId { get; set; }
    public Hotel Hotel { get; set; }

    public ICollection<Booking> Bookings { get; set; }
}