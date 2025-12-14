using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Repositories;
using PaymentGateway.Api.UseCases.Interfaces;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly PaymentsRepository _paymentsRepository;
    private readonly IPostPaymentUseCase _postPaymentUseCase;
    private readonly IValidator<PostPaymentRequest> _postPaymentValidator;

    public PaymentsController(PaymentsRepository paymentsRepository,
                              IPostPaymentUseCase postPaymentUseCase,
                              IValidator<PostPaymentRequest> postPayment)
    {
        _paymentsRepository = paymentsRepository;
        _postPaymentUseCase = postPaymentUseCase;
        _postPaymentValidator = postPayment;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = _paymentsRepository.Get(id);

        if (payment is null)
            return NotFound();

        return Ok(payment);
    }

    [HttpPost("process")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ActionResult<PostPaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync([FromBody] PostPaymentRequest request)
    {
        var validation = _postPaymentValidator.Validate(request);

        if (!validation.IsValid)
            return BadRequest(new RejectedPaymentResponse(validation.Errors));

        var result = await _postPaymentUseCase.ExecuteAsync(request);

        if(result.IsSuccess)
            return Ok(result.Value);

        return UnprocessableEntity(result.Errors);
    }
}