namespace TravelEase.Application.Features.Hotel
{
    public class CreateHotelCommand
    {
        public string Name { get; set; }
        public int CityId { get; set; }
        public string Owner { get; set; }
        public string Location { get; set; }
        public int StarRating { get; set; }
        public string Description { get; set; }
    }
}