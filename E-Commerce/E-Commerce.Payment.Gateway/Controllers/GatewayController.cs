using System.Net;
using E_Commerce.Payment.Gateway.Models;
using E_Commerce.Payment.Gateway.Repository;
using E_Commerce.Payment.Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Payment.Gateway.Controllers;

[Route("api/gateway")]
public class GatewayController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAcquiringBankService _acquiringBankService;
    private readonly IGatewayRepository _repository;

    public GatewayController(
        IHttpContextAccessor httpContextAccessor, IAcquiringBankService acquiringBankService, IGatewayRepository repository)
    {
        _httpContextAccessor = httpContextAccessor;
        _acquiringBankService = acquiringBankService;
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PaymentRequestDto paymentRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (status, message, paymentResponseDto) = await _acquiringBankService.MakePayment(paymentRequestDto);
        if (status != HttpStatusCode.Created)
            return StatusCode((int)status, message);

        await _repository.SavePayment(paymentResponseDto, paymentRequestDto);

        return Created(GeneratePaymentInfoUrl(paymentResponseDto.PaymentId), paymentResponseDto);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var (exists, payment) = await _repository.RetrievePaymentById(id);
        if (!exists)
            return NotFound("payment with id does not exists");

        return Ok(payment);
    }

    private string GeneratePaymentInfoUrl(string id)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var appBaseUrl = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}{httpContext?.Request.PathBase}";
        var routeName = ControllerContext?.ActionDescriptor?.AttributeRouteInfo?.Template;

        return $"{appBaseUrl}/{routeName}/{id}";
    }
}