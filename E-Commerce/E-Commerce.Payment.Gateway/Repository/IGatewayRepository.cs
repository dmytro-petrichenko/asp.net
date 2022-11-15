using E_Commerce.Payment.Gateway.Models;

namespace E_Commerce.Payment.Gateway.Repository;

public interface IGatewayRepository
{
    Task SavePayment(PaymentResponseDto paymentResponseDto, PaymentRequestDto paymentRequestDto);
    Task<(bool, PaymentResponseDto?)> RetrievePaymentById(string id);
}