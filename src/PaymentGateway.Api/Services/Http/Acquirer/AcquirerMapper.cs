using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Http.Acquirer.Dtos;

namespace PaymentGateway.Api.Services.Http.Acquirer;

public static class AcquirerMapper
{
    public static CreatePaymentRequest MapToAcquirerRequest(this PostPaymentRequest request)
    {
        return  new CreatePaymentRequest
        {
            CardNumber = request.CardNumber,
            ExpiryDate = $"{request.ExpiryMonth:D2}/{request.ExpiryYear}",
            Currency = request.Currency,
            Amount = request.Amount,
            Cvv = request.Cvv.ToString()
        };
    }
}
