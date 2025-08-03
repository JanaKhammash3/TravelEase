using QuestPDF.Infrastructure;
using TravelEase.Application.DTOs;
using TravelEase.Infrastructure.Services;

namespace TravelEase.Tests.UserUnitTests
{
    public class PdfReceiptGeneratorTests
    {
        public PdfReceiptGeneratorTests()
        {
            //Required for QuestPDF to run without license error
            QuestPDF.Settings.License = LicenseType.Community;
        }

        [Fact]
        public void GenerateBookingReceipt_Returns_NonEmpty_Pdf()
        {
            // Arrange
            var dto = new BookingConfirmationDto
            {
                ConfirmationNumber = "ABC123",
                HotelName = "Palm Hotel",
                HotelAddress = "123 Beach Rd",
                RoomDetails = "Deluxe Suite",
                CheckIn = new DateTime(2025, 7, 10),
                CheckOut = new DateTime(2025, 7, 15),
                TotalPrice = 500.75m,
                PaymentStatus = "Paid"
            };

            // Act
            var pdfBytes = PdfReceiptGenerator.GenerateBookingReceipt(dto);

            // Assert
            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 100, "Generated PDF should not be too small.");
        }
    }
}