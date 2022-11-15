using E_Commerce.Payment.Gateway.Models;

namespace E_Commerce.Payment.Gateway.Services;

public interface IAcquiringBankService
{
    Task<PaymentResult> MakePayment(PaymentRequestDto paymentRequestDto);
}