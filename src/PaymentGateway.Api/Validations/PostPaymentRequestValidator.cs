using FluentValidation;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validations;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    public PostPaymentRequestValidator()
    {
        //CardNumber
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required.")
            .Length(14, 19).WithMessage("Card number must be between 14 and 19 characters long.")
            .Matches(@"^\d+$").WithMessage("Card number must only contain numeric characters.");

        //ExpiryMonth
        RuleFor(x => x.ExpiryMonth)
            .NotEmpty().WithMessage("Expiry month is required.")
            .InclusiveBetween(1, 12).WithMessage("Expiry month must be between 1 and 12.");

        RuleFor(x => x.ExpiryMonth)
            .GreaterThan(DateTime.Now.Month).WithMessage("The expiry date must be in the future")
            .When(x => x.ExpiryYear == DateTime.Now.Year);

        //ExpiryYear
        RuleFor(x => x.ExpiryYear)
            .NotEmpty().WithMessage("Expiry year is required.")
            .GreaterThanOrEqualTo(DateTime.Now.Year).WithMessage("Expiry year must be in the current year or the future.");

        //Currency
        RuleFor(x => x.Currency)
            .NotNull().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be 3 characters long (ISO code).")
            .Matches(@"^(USD|EUR|GBP)$")
            .WithMessage("Currency is invalid. Must be one of: USD, EUR, GBP.");

        //Amount
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount is required.")
            .InclusiveBetween(1, int.MaxValue)
            .WithMessage("Amount must be a positive integer representing the minor currency unit.");

        //CVV
        RuleFor(x => x.Cvv)
            .NotEmpty().WithMessage("CVV is required.")
            .Length(3,4).WithMessage("CVV must be 3 or 4 characters long.")
            .Matches(@"^\d+$").WithMessage("CVV must only contain numeric characters.");
    }
}
