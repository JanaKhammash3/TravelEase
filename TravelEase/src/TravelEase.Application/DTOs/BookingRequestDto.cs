using TravelEase.TravelEase.Domain.Enums;

namespace TravelEase.TravelEase.Application.DTOs
{
    public class BookingRequestDto
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public int Adults { get; set; }
        public int Children { get; set; }

        public string SpecialRequests { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Card;

        public bool SimulatePaymentSuccess { get; set; } = true;
    }
}