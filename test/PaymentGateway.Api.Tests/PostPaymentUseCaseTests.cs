using System.Net;
using AutoBogus;
using Moq;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Http.Acquirer;
using PaymentGateway.Api.Services.Http.Acquirer.Dtos;
using PaymentGateway.Api.Services.Repositories;
using PaymentGateway.Api.UseCases;
using RestEase;

namespace PaymentGateway.Api.Tests;
public class PostPaymentUseCaseTests
{
    private PostPaymentUseCase _useCase;

    private readonly PaymentsRepository _paymentsRepository = new();
    private readonly Mock<IAcquirerApi> _acquirerApiMock = new();

    public PostPaymentUseCaseTests()
    {
        _useCase = new PostPaymentUseCase(_paymentsRepository, _acquirerApiMock.Object);
    }

    private Response<T> Generate<T>(T response, HttpStatusCode statusCode)
    {
        return new Response<T>("", new HttpResponseMessage(statusCode), () => response);
    }

    [Fact]
    public async Task ExecuteAsync_IsSuccessful_When_CreatePaymentAsync_Returns_SuccessfulStatusCode()
    {
        //Arrange
        var request = AutoFaker.Generate<PostPaymentRequest>();

        var response = Generate(new CreatePaymentResponse
        {
            AuthorizationCode = Guid.NewGuid(),
            Authorized = true

        }, HttpStatusCode.OK);

        _acquirerApiMock.Setup(api => api.CreatePaymentAsync(request.MapToAcquirerRequest()))
            .ReturnsAsync(response);

        //Act
        var result = await _useCase.ExecuteAsync(request);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Currency, request.Currency);
        Assert.Equal(result.Value.CardNumberLastFour, request.CardNumber[^4..]);
        Assert.Equal(result.Value.ExpiryMonth, request.ExpiryMonth);
        Assert.Equal(result.Value.ExpiryYear, request.ExpiryYear);
        Assert.Equal(result.Value.Amount, request.Amount);
        Assert.Equal(result.Value.Id, response.GetContent().AuthorizationCode);
        Assert.Equal(PaymentStatus.Authorized, result.Value.Status);

        _acquirerApiMock.Verify(x => x.CreatePaymentAsync(It.IsAny<CreatePaymentRequest>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Fails_When_CreatePaymentAsync_Fails()
    {
        //Arrange
        var request = AutoFaker.Generate<PostPaymentRequest>();

        var response = Generate(new CreatePaymentResponse(), HttpStatusCode.BadGateway);

        _acquirerApiMock.Setup(api => api.CreatePaymentAsync(request.MapToAcquirerRequest()))
            .ReturnsAsync(response);

        //Act
        var result = await _useCase.ExecuteAsync(request);

        //Assert
        Assert.True(result.IsFailed);

        _acquirerApiMock.Verify(x => x.CreatePaymentAsync(It.IsAny<CreatePaymentRequest>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_IsSuccessful_When_CreatePaymentAsync_Returns_SuccessfulStatusCode_But_Is_Declined()
    {
        //Arrange
        var request = AutoFaker.Generate<PostPaymentRequest>();

        var response = Generate(new CreatePaymentResponse
        {
            AuthorizationCode = null,
            Authorized = false

        }, HttpStatusCode.OK);

        _acquirerApiMock.Setup(api => api.CreatePaymentAsync(request.MapToAcquirerRequest()))
            .ReturnsAsync(response);

        //Act
        var result = await _useCase.ExecuteAsync(request);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Currency, request.Currency);
        Assert.Equal(result.Value.CardNumberLastFour, request.CardNumber[^4..]);
        Assert.Equal(result.Value.ExpiryMonth, request.ExpiryMonth);
        Assert.Equal(result.Value.ExpiryYear, request.ExpiryYear);
        Assert.Equal(result.Value.Amount, request.Amount);
        Assert.Equal(PaymentStatus.Declined, result.Value.Status);

        _acquirerApiMock.Verify(x => x.CreatePaymentAsync(It.IsAny<CreatePaymentRequest>()), Times.Once);
    }
}
