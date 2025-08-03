namespace TravelEase.TravelEase.Application.Features.Booking;

public class CreateBookingCommand
{
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string SpecialRequests { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
}