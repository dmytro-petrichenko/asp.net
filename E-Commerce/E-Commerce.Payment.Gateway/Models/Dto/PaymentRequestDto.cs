using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Payment.Gateway.Models;

public class PaymentRequestDto
{
    [CreditCard(ErrorMessage = "Valid card number is required")]
    public string Number { get; set; }

    [Required(ErrorMessage = "ExpiryMonth is required")]
    [Range(1, 12, ErrorMessage = "Value for {0} must be between {1} and {2}")]
    public int ExpiryMonth { get; set; }

    [Required(ErrorMessage = "ExpiryYear is required")]
    [Range(2023, 2123, ErrorMessage = "Value for {0} must be between {1} and {2}")]
    public int ExpiryYear { get; set; } = default!;

    [Required(ErrorMessage = "Card cvv is required")]
    public string Cvv { get; set; }

    [Required(ErrorMessage = "Type is required")]
    public string Type { get; set; } = default!;

    [Required(ErrorMessage = "Name on card is required")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Currency is required")]
    public string Currency { get; set; } = default!;

    [Required(ErrorMessage = "Amount is required")]
    [Range(0, 1000_000, ErrorMessage = "Value for {0} must be between {1} and {2}")]
    public int Amount { get; set; }
}

