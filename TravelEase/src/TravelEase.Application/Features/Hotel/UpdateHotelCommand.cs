namespace TravelEase.TravelEase.Application.Features.Hotel
{
    public class UpdateHotelCommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public string Owner { get; set; }
        public string Location { get; set; }
        public int StarRating { get; set; }
        public string Description { get; set; }
    }
}