namespace E_Commerce.Payment.Gateway.Models;

public class PaymentResponseDto
{
    public string PaymentId { get; set; } = default!;
    public string PaymentStatus { get; set; } = default!;
    public string Last4 { get; set; } = default!;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Name { get; set; } = default!;
    public string Currency { get; set; } = default!;
    public long Amount { get; set; }
    public DateTime? ProcessedOn { get; set; }
}