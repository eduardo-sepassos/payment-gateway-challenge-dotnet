using FluentResults;
using RestEase;

namespace PaymentGateway.Api.Extensions;

public static class HttpResponseExtensions
{
    public static Result<T> Validate<T>(this Response<T> response)
    {
        if (response.ResponseMessage.IsSuccessStatusCode)
        {
            return Result.Ok(response.GetContent());
        }

        var statusCode = response.ResponseMessage.StatusCode;

        return Result.Fail(new Error($"{statusCode} response received from acquirer")
            .WithMetadata("HttpStatusCode", (int)statusCode));
    }
}
