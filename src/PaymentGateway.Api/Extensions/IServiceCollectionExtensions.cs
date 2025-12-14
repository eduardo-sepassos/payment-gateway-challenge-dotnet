using FluentValidation;

using PaymentGateway.Api.Configuration;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Http.Acquirer;
using PaymentGateway.Api.UseCases;
using PaymentGateway.Api.UseCases.Interfaces;
using PaymentGateway.Api.Validations;

using Polly;

using RestEase.HttpClientFactory;

namespace PaymentGateway.Api.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPostPaymentUseCase, PostPaymentUseCase>();

        var acquirerConfiguration = configuration.GetSection(nameof(AcquirerConfiguration))
            .Get<AcquirerConfiguration>();

        if(acquirerConfiguration is null)
            throw new ArgumentNullException($"{nameof(AcquirerConfiguration)} cannot be null");

        services
            .AddRestEaseClient<IAcquirerApi>(acquirerConfiguration.BaseUrl)
            .AddTransientHttpErrorPolicy(builder => builder
                .WaitAndRetryAsync([
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(7),
                    ])
            );

        services.AddScoped<IValidator<PostPaymentRequest>, PostPaymentRequestValidator>();

        return services;
    }
}
