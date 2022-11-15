using AutoMapper;
using Checkout.Common;
using Checkout.Payments.Request;
using Checkout.Payments.Response;
using Checkout.Payments.Response.Source;
using Checkout.Tokens;
using E_Commerce.Payment.Gateway.Models;

namespace E_Commerce.Payment.Gateway;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<PaymentRequestDto, CardTokenRequest>().ReverseMap();
            config.CreateMap<PaymentRequestDto, CardDetails>();
            config.CreateMap<PaymentResponseDto, PaymentResponse>();
            config.CreateMap<PaymentRequestDto, PaymentRequest>().ForMember(d => d.Currency,
                op => op.MapFrom(o => MapStringToCurrency(o.Currency)));
            config.CreateMap<PaymentResponseDto, PaymentDetails>()
                .ForMember(p => p.Status,
                    op => op.MapFrom(src =>src.PaymentStatus));
            config.CreateMap<PaymentResponse, PaymentResponseDto>()
                .IncludeMembers(p=> p.Source as CardResponseSource)
                .ForMember(p => p.PaymentStatus,
                    opt => opt.MapFrom(src => src.Status.ToString()));
            config.CreateMap<CardResponseSource, PaymentResponseDto>()
                .ForMember(d => d.PaymentId,
                    opt => opt.MapFrom(src => src.Id));
            config.CreateMap<PaymentDetails, PaymentResponseDto>()
                .IncludeMembers(p => p.CardDetails)
                .ForMember(p => p.PaymentStatus,
                    op => op.MapFrom(src =>src.Status));
            config.CreateMap<CardDetails, PaymentResponseDto>().ForMember(d => d.Last4,
                op => op.MapFrom(o => CardNumberToLast4(o.Number)));
        });

        return mappingConfig;
    }

    static string CardNumberToLast4(string number)
    {
        return number[^4..];
    }

    static Currency? MapStringToCurrency(string? currency)
    {
        if (Enum.TryParse(currency, true, out Currency c))
            return c;

        return default;
    }
}