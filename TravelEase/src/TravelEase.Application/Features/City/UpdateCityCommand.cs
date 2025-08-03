namespace TravelEase.TravelEase.Application.Features.City
{
    public class UpdateCityCommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string PostOffice { get; set; }
    }
}