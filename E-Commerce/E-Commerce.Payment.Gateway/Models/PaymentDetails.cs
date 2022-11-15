using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Payment.Gateway.Models;

[Index(nameof(PaymentId))]
public class PaymentDetails
{
    [Key]
    public int Id { get; set; }

    public string PaymentId { get; set; } = default!;

    public int CardDetailsId { get; set; }

    [ForeignKey("CardDetailsId")]
    public CardDetails CardDetails { get; set; } = default!;

    public string Status { get; set; } = default!;

    public string Currency { get; set; } = default!;

    public int Amount { get; set; }

    public DateTime? ProcessedOn { get; set; }
}