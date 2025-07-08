using TravelEase.TravelEase.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }

    public User User { get; set; }
    public Room Room { get; set; }
}