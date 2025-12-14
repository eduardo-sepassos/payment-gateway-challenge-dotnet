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

        return Result.Fail($"{response.ResponseMessage.StatusCode} response received from acquirer");
    }
}
