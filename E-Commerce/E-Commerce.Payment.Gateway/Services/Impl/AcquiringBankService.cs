using System.Net;
using AutoMapper;
using Checkout;
using Checkout.Payments.Request;
using Checkout.Payments.Request.Source;
using Checkout.Payments.Response;
using Checkout.Tokens;
using E_Commerce.Payment.Gateway.Models;
using Model = E_Commerce.Payment.Gateway.Models;

namespace E_Commerce.Payment.Gateway.Services.Impl;

public class AcquiringBankService : IAcquiringBankService
{
    private readonly ICheckoutApi _api;
    private readonly IMapper _mapper;

    public AcquiringBankService(ICheckoutApi api, IMapper mapper)
        => (_api, _mapper) = (api, mapper);

    public async Task<PaymentResult> MakePayment(PaymentRequestDto paymentRequestDto)
    {
        var (code, message, token) = await GetCardToken(paymentRequestDto);
        if (code != HttpStatusCode.Created)
            return new (code, message, default);

        (code, message, var data) = await RequestPayment(token, paymentRequestDto);
        if (code != HttpStatusCode.Created)
            return new (code, message, default);

        var res = _mapper.Map<PaymentResponseDto>(data);

        return new (code, message, res);
    }

    private async Task<(HttpStatusCode code, string message, string token)> GetCardToken(
        PaymentRequestDto paymentRequestDto)
    {
        var request = _mapper.Map<CardTokenRequest>(paymentRequestDto);

        try
        {
            CardTokenResponse response = await _api.TokensClient().Request(request);
            return (HttpStatusCode.Created, String.Empty, response.Token);
        }
        catch (CheckoutApiException e)
        {
            return (e.HttpStatusCode,
                string.Join(", ", e.ErrorDetails["error_codes"]), string.Empty);
        }
        catch (CheckoutArgumentException e)
        {
            return (HttpStatusCode.BadRequest, e.Message, string.Empty);
        }
        catch (CheckoutAuthorizationException e)
        {
            return (HttpStatusCode.Unauthorized, e.Message, string.Empty);
        }
    }

    private async Task<(HttpStatusCode code, string message, PaymentResponse? data)> RequestPayment(string token, PaymentRequestDto paymentRequestDto)
    {
        var request = _mapper.Map<PaymentRequest>(paymentRequestDto);
        request.Source = new RequestTokenSource() { Token = token };
        request.ProcessingChannelId = "pc_vmec3h2tfppu5p7mb6fxmliyhi";
        request.CaptureOn = DateTime.Now;

        try
        {
            var response = await _api.PaymentsClient().RequestPayment(request);
            return (HttpStatusCode.Created, String.Empty, response);
        }
        catch (CheckoutApiException e)
        {
            return (e.HttpStatusCode, string.Join(", ", e.ErrorDetails["error_codes"]), default);
        }
        catch (CheckoutArgumentException e)
        {
            return (HttpStatusCode.BadRequest, e.Message, default);
        }
        catch (CheckoutAuthorizationException e)
        {
            return (HttpStatusCode.Unauthorized, e.Message, default);
        }
    }
}