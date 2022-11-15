using AutoMapper;
using E_Commerce.Payment.Gateway.DataBase;
using E_Commerce.Payment.Gateway.Models;
using E_Commerce.Payment.Gateway.Repository.Impl;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Payment.Gateway.Tests.Integration;

public class GatewayRepositoryTests : IDisposable
{
    private GatewayRepository _sut;
    private Database _dbContext;
    private IMapper _mapper;

    public GatewayRepositoryTests()
    {
        _mapper = MappingConfig.RegisterMaps().CreateMapper();
        _dbContext = CreateDbContext();
        _sut = new GatewayRepository(_dbContext, _mapper);
    }

    [Fact]
    public async Task RetrievePaymentById_WhenExistingId_ShouldReturnValue()
    {
        // Act
        var (success, dto) = await _sut.RetrievePaymentById("someid");

        // Assert
        success.ShouldBe(true);
        dto.ShouldNotBeNull();
        dto.PaymentId.ShouldBe("someid");
    }

    [Fact]
    public async Task RetrievePaymentById_WhenNotExistingId_ShouldReturnFalse()
    {
        // Act
        var (success, dto) = await _sut.RetrievePaymentById("wrongid");

        // Assert
        success.ShouldBe(false);
        dto.ShouldBe(null);
    }

    private Database CreateDbContext()
    {
        var sqliteConnection = new SqliteConnection("Filename=:memory:");
        sqliteConnection.Open();
        var options = new DbContextOptionsBuilder<Database>().UseSqlite(sqliteConnection).Options;

        var context = new Database(options);
        context.Database.EnsureCreated();

        context.Add(new CardDetails()
        {
            Id = 1,
            Number = "2514",
            ExpiryMonth = 12,
            ExpiryYear = 2120,
            Name = "Test"
        });

        context.Add(new PaymentDetails()
        {
            Id = 1,
            PaymentId = "someid",
            CardDetailsId = 1,
            Amount = 100,
            Status = "",
            Currency = "",
            ProcessedOn = new DateTime()

        });

        context.Add(new PaymentDetails()
        {
            Id = 2,
            PaymentId = "someid2",
            CardDetailsId = 1,
            Amount = 110,
            Status = "",
            Currency = "",
            ProcessedOn = new DateTime()
        });

        context.SaveChanges();

        return context;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}