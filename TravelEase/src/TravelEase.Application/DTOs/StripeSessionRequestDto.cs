namespace TravelEase.TravelEase.Application.DTOs;

public class StripeSessionRequestDto
{
    public long AmountCents { get; set; }
    public string ProductName { get; set; }
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}