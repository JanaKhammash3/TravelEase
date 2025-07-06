namespace TravelEase.TravelEase.Domain.Entities;

public class City
{
    public int Id { get; set; }  // ✅ Must match Hotel's `CityId`
    public string Name { get; set; }
    public string Country { get; set; }
    public string PostOffice { get; set; }

    public List<Hotel> Hotels { get; set; } = new();
}
