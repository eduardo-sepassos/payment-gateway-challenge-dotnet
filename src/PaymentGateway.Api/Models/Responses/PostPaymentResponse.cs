using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Http.Acquirer.Dtos;

namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }

    public PostPaymentResponse()
    {
        
    }

    public PostPaymentResponse(CreatePaymentResponse createPaymentResponse, PostPaymentRequest postPaymentRequest)
    {
        Id = createPaymentResponse.Authorized ? createPaymentResponse.AuthorizationCode.GetValueOrDefault() : Guid.NewGuid();
        Status = createPaymentResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
        CardNumberLastFour = postPaymentRequest.CardNumber[^4..];
        ExpiryMonth = postPaymentRequest.ExpiryMonth;
        ExpiryYear = postPaymentRequest.ExpiryYear;
        Currency = postPaymentRequest.Currency;
        Amount = postPaymentRequest.Amount;
    }
}
