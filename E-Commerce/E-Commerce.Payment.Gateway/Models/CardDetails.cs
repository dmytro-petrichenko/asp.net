using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Payment.Gateway.Models;

[Index(nameof(Number))]
public class CardDetails
{
    [Key]
    public int Id { get; set; }

    public string Number { get; set; } = default!;

    public int ExpiryMonth { get; set; }

    public int ExpiryYear { get; set; }

    public string Name { get; set; } = default!;
}