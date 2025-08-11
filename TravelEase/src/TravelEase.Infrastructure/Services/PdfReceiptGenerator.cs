using QuestPDF.Fluent;
using QuestPDF.Helpers;
using TravelEase.TravelEase.Application.DTOs;

namespace TravelEase.TravelEase.Infrastructure.Services
{
    public static class PdfReceiptGenerator
    {
        public static byte[] GenerateBookingReceipt(BookingConfirmationDto dto)
        {
            using var stream = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Header().Text("TravelEase Booking Confirmation").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Confirmation #: {dto.ConfirmationNumber}");
                        col.Item().Text($"Hotel: {dto.HotelName}");
                        col.Item().Text($"Address: {dto.HotelAddress}");
                        col.Item().Text($"Room: {dto.RoomDetails}");
                        col.Item().Text($"Dates: {dto.CheckIn:yyyy-MM-dd} → {dto.CheckOut:yyyy-MM-dd}");
                        col.Item().Text($"Total: ${dto.TotalPrice}");
                        col.Item().Text($"Payment Status: {dto.PaymentStatus}");
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Thank you for booking with TravelEase!").FontSize(12);
                    });
                });
            }).GeneratePdf(stream);

            return stream.ToArray();
        }
    }
}