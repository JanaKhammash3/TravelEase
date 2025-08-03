namespace TravelEase.Application.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string HotelName { get; set; }
        public string RoomNumber { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; }
    }
}