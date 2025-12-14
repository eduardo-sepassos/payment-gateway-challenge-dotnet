namespace PaymentGateway.Api.Models.Responses;

public class RejectedPaymentResponse
{
    public PaymentStatus Status { get;}
    public object Errors { get; init; }

    public RejectedPaymentResponse(object errors)
    {
        Status = PaymentStatus.Rejected;
        Errors = errors;
    }
}
