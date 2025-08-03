using Microsoft.AspNetCore.Mvc;
using TravelEase.Application.DTOs;
using TravelEase.Application.Interfaces;
using TravelEase.Infrastructure.Services;

namespace TravelEase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunicationController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public CommunicationController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailDto dto)
        {
            await _emailService.SendEmailAsync(dto.To, dto.Subject, dto.Body);
            return Ok("Email sent successfully.");
        }

        [HttpPost("send-email-with-pdf")]
        public async Task<IActionResult> SendEmailWithPdf([FromBody] BookingConfirmationDto dto)
        {
            var pdf = PdfReceiptGenerator.GenerateBookingReceipt(dto);
            string subject = "Your Booking Confirmation";
            string body = "Please find attached your booking confirmation.";

            await _emailService.SendEmailWithAttachmentAsync(dto.UserEmail, subject, body, pdf, "receipt.pdf");
            return Ok("Email with PDF sent.");
        }

        [HttpPost("generate-pdf")]
        public IActionResult GeneratePdf([FromBody] BookingConfirmationDto dto)
        {
            var pdf = PdfReceiptGenerator.GenerateBookingReceipt(dto);
            return File(pdf, "application/pdf", "BookingReceipt.pdf");
        }
    }

    // Helper DTO for simple email
    public class EmailDto
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}