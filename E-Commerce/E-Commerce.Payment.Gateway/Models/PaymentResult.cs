using System.Net;

namespace E_Commerce.Payment.Gateway.Models;

public record PaymentResult(HttpStatusCode Code, string Message, PaymentResponseDto? Data);