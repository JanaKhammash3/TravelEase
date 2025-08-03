namespace TravelEase.Domain.Entities;

public class HotelView
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int HotelId { get; set; }
    public DateTime ViewedAt { get; set; }

    public User? User { get; set; }
    public Hotel? Hotel { get; set; }
}