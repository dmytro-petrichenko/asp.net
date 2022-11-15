using System.Net;
using E_Commerce.Payment.Gateway.Controllers;
using E_Commerce.Payment.Gateway.Models;
using E_Commerce.Payment.Gateway.Repository;
using E_Commerce.Payment.Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Payment.Gateway.Tests.Unit;

public class GatewayControllerTests
{
    private GatewayController _sut;
    private IHttpContextAccessor _httpContextAccessor;
    private IAcquiringBankService _acquiringBankService;
    private IGatewayRepository _repository;

    public GatewayControllerTests()
    {
        _httpContextAccessor = Mock.Create<IHttpContextAccessor>();
        _acquiringBankService = Mock.Create<IAcquiringBankService>();
        _repository = Mock.Create<IGatewayRepository>();
    }

    [Fact]
    public async Task Post_WithInvalidParameters_ShouldReturnBadRequest()
    {
        // Arrange
        _sut = new GatewayController(_httpContextAccessor, _acquiringBankService, _repository);
        _sut.ModelState.AddModelError("key", "error");

        // Assert
        var result = await _sut.Post(new PaymentRequestDto());

        // Act
        result.ShouldBeOfType(typeof(BadRequestObjectResult));
    }

    [Fact]
    public async Task Post_WithValidParameters_ShouldReturnCreated()
    {
        // Arrange
        Mock.Arrange(() => _acquiringBankService.MakePayment(Arg.IsAny<PaymentRequestDto>()))
            .Returns(() => Task.FromResult(new PaymentResult(HttpStatusCode.Created, string.Empty, new PaymentResponseDto())));
        Mock.Arrange(() => _repository.SavePayment(Arg.IsAny<PaymentResponseDto>(), Arg.IsAny<PaymentRequestDto>()))
            .Returns(() => Task.CompletedTask);
        _sut = new GatewayController(_httpContextAccessor, _acquiringBankService, _repository);

        // Assert
        var result = await _sut.Post(new PaymentRequestDto());

        // Act
        result.ShouldBeOfType(typeof(CreatedResult));
    }

    [Fact]
    public async Task Post_WithProvidedParameters_ShouldCallMakePaymentWithSameParameters()
    {
        // Arrange
        Mock.Arrange(() => _acquiringBankService.MakePayment(Arg.IsAny<PaymentRequestDto>()))
            .Returns(() => Task.FromResult(new PaymentResult(HttpStatusCode.Created, string.Empty, new PaymentResponseDto())));
        Mock.Arrange(() => _repository.SavePayment(Arg.IsAny<PaymentResponseDto>(), Arg.IsAny<PaymentRequestDto>()))
            .Returns(() => Task.CompletedTask);
        _sut = new GatewayController(_httpContextAccessor, _acquiringBankService, _repository);

        // Act
        var result = await _sut.Post(ArrangePaymentRequestDtoWithNumber("3456234512334"));

        // Assert
        result.ShouldBeOfType(typeof(CreatedResult));
        Mock.Assert(() => _acquiringBankService.MakePayment(Arg.Matches<PaymentRequestDto>(p => p.Number == "3456234512334")), Occurs.Once());
    }

    [Fact]
    public async Task Get_WithValidId_ShouldReturnPaymentResponseDto()
    {
        // Arrange
        var expectedResponseDto = new PaymentResponseDto();
        Mock.Arrange(() => _repository.RetrievePaymentById("test_id"))
            .Returns(() => Task.FromResult((true, expectedResponseDto)));
        _sut = new GatewayController(_httpContextAccessor, _acquiringBankService, _repository);

        // Act
        var result = await _sut.Get("test_id");

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType(typeof(OkObjectResult));
        ((OkObjectResult)result).Value.ShouldNotBeNull();
        Assert.Equal(expectedResponseDto, ((OkObjectResult)result).Value);
    }

    private PaymentRequestDto ArrangePaymentRequestDtoWithNumber(string number)
    {
        return new PaymentRequestDto() { Number = number };
    }

}