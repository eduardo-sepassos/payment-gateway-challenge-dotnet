using FluentResults;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.UseCases.Interfaces;

public interface IPostPaymentUseCase
{
    Task<Result<PostPaymentResponse>> ExecuteAsync(PostPaymentRequest request);
}
