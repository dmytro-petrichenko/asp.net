using AutoMapper;
using E_Commerce.Payment.Gateway.DataBase;
using E_Commerce.Payment.Gateway.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Payment.Gateway.Repository.Impl;

public class GatewayRepository : IGatewayRepository
{
    private Database _db;
    private IMapper _mapper;

    public GatewayRepository(Database db, IMapper mapper) => (_db, _mapper) = (db, mapper);

    public async Task SavePayment(PaymentResponseDto paymentResponseDto, PaymentRequestDto paymentRequestDto)
    {
        var cardInfo = _mapper.Map<CardDetails>(paymentRequestDto);
        var payment = _mapper.Map<PaymentDetails>(paymentResponseDto);

        cardInfo = await AddCardIfNotExists(cardInfo);
        payment.CardDetails = cardInfo;
        await AddPayment(payment);

        await _db.SaveChangesAsync();
    }

    public async Task<(bool, PaymentResponseDto?)> RetrievePaymentById(string id)
    {
        var payment = await _db.PaymentDetails
            .Include(p => p.CardDetails)
            .SingleOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null)
            return (false, null);

        var result = _mapper.Map<PaymentResponseDto>(payment);

        return (true, result);
    }

    private async ValueTask AddPayment(PaymentDetails paymentDetails)
    {
        await _db.AddAsync(paymentDetails);
    }

    private async ValueTask<CardDetails> AddCardIfNotExists(CardDetails cardDetails)
    {
        var card = await _db.CardDetails.SingleOrDefaultAsync(c => c.Number == cardDetails.Number);
        if (card != default)
            return card;

        var entry = await _db.AddAsync(cardDetails);
        return entry.Entity;
    }
}