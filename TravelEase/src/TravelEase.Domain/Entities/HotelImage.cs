using TravelEase.TravelEase.Domain.Entities;

public class HotelImage
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string ImageUrl { get; set; }

    public Hotel Hotel { get; set; } 
}