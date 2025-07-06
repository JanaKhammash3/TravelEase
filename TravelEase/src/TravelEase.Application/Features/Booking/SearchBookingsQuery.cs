namespace TravelEase.TravelEase.Application.Features.Booking;

public class SearchBookingsQuery
{
    public int? UserId { get; set; }
    public int? RoomId { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
}
