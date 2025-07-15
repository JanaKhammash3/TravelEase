using Microsoft.AspNetCore.Http;

namespace TravelEase.TravelEase.API.Models
{
    public class HotelImageUploadDto
    {
        public int HotelId { get; set; }
        public List<IFormFile> Images { get; set; } = new();
    }
}