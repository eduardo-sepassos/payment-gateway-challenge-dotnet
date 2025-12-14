using FluentResults;
using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Http.Acquirer;
using PaymentGateway.Api.Services.Repositories;
using PaymentGateway.Api.UseCases.Interfaces;

namespace PaymentGateway.Api.UseCases;

public class PostPaymentUseCase : IPostPaymentUseCase
{
    private readonly PaymentsRepository _paymentsRepository;
    private readonly IAcquirerApi _acquirerApi;
    public PostPaymentUseCase(PaymentsRepository paymentsRepository, IAcquirerApi acquirerApi)
    {
        _paymentsRepository = paymentsRepository;
        _acquirerApi = acquirerApi;
    }
    public async Task<Result<PostPaymentResponse>> ExecuteAsync(PostPaymentRequest request)
    {
        var response = await _acquirerApi.CreatePaymentAsync(request.MapToAcquirerRequest());

        var result = response.Validate();

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        var paymentResponse = new PostPaymentResponse(result.Value, request);

        _paymentsRepository.Add(paymentResponse);

        return Result.Ok(paymentResponse);
    }
}
