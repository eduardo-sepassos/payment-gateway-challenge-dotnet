using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoBogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Repositories;
using PaymentGateway.Api.UseCases.Interfaces;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };
    private readonly Mock<IPostPaymentUseCase> _mockPostPasymentUseCase = new();

    private HttpClient CreateClient(PaymentsRepository paymentsRepository)
    {
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        return webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();
    }

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(0, 9999).ToString().PadLeft(4, '0'),
            Currency = "GBP"
        };

        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.Add(payment);

        var client = CreateClient(paymentsRepository);

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(_jsonSerializerOptions);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostPayment_Executes_Successfully()
    {
        //Arrange
        var request = new AutoFaker<PostPaymentRequest>()
            .RuleFor(x => x.CardNumber, "2222405343248877")
            .RuleFor(x => x.ExpiryMonth, 4)
            .RuleFor(x => x.ExpiryYear, 2026)
            .RuleFor(x => x.Currency, "USD")
            .RuleFor(X => X.Amount, 1050)
            .RuleFor(x => x.Cvv, "1234")
            .Generate();

        var client = CreateClient(new PaymentsRepository());

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(request, options: _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");

        //Act
        var response = await client.PostAsync("/api/Payments/process", jsonContent);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(_jsonSerializerOptions);

        //Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task PostPayment_Returns_BadRequest_Case_Invalid_Request()
    {
        //Arrange
        var request = new AutoFaker<PostPaymentRequest>()
            .RuleFor(x => x.CardNumber, "2222405343248877")
            .RuleFor(x => x.ExpiryMonth, 4)
            .RuleFor(x => x.ExpiryYear, 2026)
            .RuleFor(x => x.Currency, "USD")
            .RuleFor(X => X.Amount, 1050)
            .RuleFor(x => x.Cvv, "")
            .Generate();

        var client = CreateClient(new PaymentsRepository());

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(request, options: _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");

        //Act
        var response = await client.PostAsync("/api/Payments/process", jsonContent);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}