using Newtonsoft.Json;

namespace PaymentGateway.Api.Services.Http.Acquirer.Dtos;

public record CreatePaymentResponse
{
    [JsonProperty("authorized")]
    public bool Authorized { get; init; }

    [JsonProperty("authorization_code")]
    public Guid? AuthorizationCode { get; init; }
}
