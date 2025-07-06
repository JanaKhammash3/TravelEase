namespace TravelEase.TravelEase.Application.Features.Booking;

public class UpdateBookingCommand
{
    public int Id { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string SpecialRequests { get; set; }
}