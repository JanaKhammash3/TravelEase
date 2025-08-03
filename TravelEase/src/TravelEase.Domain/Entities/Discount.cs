namespace TravelEase.Domain.Entities;

public class Discount
{
    public int Id { get; set; } 
    public string Code { get; set; }
    public decimal Percentage { get; set; }

    public int? HotelId { get; set; }
    public Hotel Hotel { get; set; }

    public int? RoomId { get; set; }
    public Room Room { get; set; }
}