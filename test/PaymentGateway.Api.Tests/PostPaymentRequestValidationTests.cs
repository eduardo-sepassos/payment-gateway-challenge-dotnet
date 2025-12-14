using FluentValidation.TestHelper;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validations;

namespace PaymentGateway.Api.Tests;
public class PostPaymentRequestValidationTests
{
    private readonly PostPaymentRequestValidator _validator = new();

    private PostPaymentRequest GetValidRequest()
    {
        return new PostPaymentRequest
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = DateTime.Now.Month,
            ExpiryYear = DateTime.Now.Year + 1,
            Currency = "USD",
            Amount = 1000,
            Cvv = "123"
        };
    }

    [Fact]
    public void Should_Pass_For_All_Valid_Properties()
    {
        var request = GetValidRequest();

        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("1234567890123")]
    [InlineData("12345678901234567890")]
    [InlineData("1234567890ABCD")]
    public void CardNumber_Should_Fail_When_Invalid(string cardNumber)
    {
        var request = GetValidRequest();
        request.CardNumber = cardNumber;

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.CardNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void ExpiryMonth_Should_Fail_When_Outside_Range(int month)
    {
        var request = GetValidRequest();
        request.ExpiryMonth = month;
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.ExpiryMonth);
    }

    [Fact]
    public void ExpiryYear_Should_Fail_When_In_The_Past()
    {
        var request = GetValidRequest();
        request.ExpiryYear = DateTime.Now.Year - 1;
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.ExpiryYear);
    }

    [Fact]
    public void ExpiryMonth_Should_Fail_When_Year_Is_Current_And_Month_Is_In_The_Past()
    {
        var request = GetValidRequest();
        request.ExpiryYear = DateTime.Now.Year;
        request.ExpiryMonth = DateTime.Now.Month - 1;
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.ExpiryMonth);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("US")]
    [InlineData("USDO")]
    [InlineData("AUD")]
    public void Currency_Should_Fail_When_Invalid(string currency)
    {
        var request = GetValidRequest();
        request.Currency = currency;

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("12")]
    [InlineData("12345")]
    [InlineData("1A3")]
    public void CVV_Should_Fail_When_Invalid(string cvv)
    {
        var request = GetValidRequest();
        request.Cvv = cvv;

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Cvv);
    }
}
