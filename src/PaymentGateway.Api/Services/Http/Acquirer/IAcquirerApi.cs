using PaymentGateway.Api.Services.Http.Acquirer.Dtos;
using RestEase;

namespace PaymentGateway.Api.Services.Http.Acquirer;

public interface IAcquirerApi
{
    [Post("/payments")]
    [AllowAnyStatusCode]
    Task<Response<CreatePaymentResponse>> CreatePaymentAsync([Body] CreatePaymentRequest request);
}
