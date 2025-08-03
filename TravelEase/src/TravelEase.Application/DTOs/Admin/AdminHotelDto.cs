namespace TravelEase.Application.DTOs.Admin;

public class AdminHotelDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int StarRating { get; set; }
    public string Owner { get; set; }
    public int RoomCount { get; set; }
    public string CityName { get; set; }

    public string Location { get; set; }
    public string Amenities { get; set; }
}