// RoomDto.cs
namespace TravelEase.Application.DTOs.Admin;

public class AdminRoomDto {
    public int Id { get; set; }
    public string Number { get; set; }
    public bool IsAvailable { get; set; }
    public int CapacityAdults { get; set; }
    public int CapacityChildren { get; set; }
    public string HotelName { get; set; }
}