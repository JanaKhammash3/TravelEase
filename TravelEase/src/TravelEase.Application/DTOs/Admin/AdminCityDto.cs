// CityDto.cs
namespace TravelEase.TravelEase.Application.DTOs.Admin;

public class AdminCityDto {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public string PostOffice { get; set; }
    public int HotelCount { get; set; }
}