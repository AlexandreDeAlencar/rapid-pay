using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RapidPay.PaymentFees.App.Common.Behaviors;
using System.Reflection;

namespace RapidPay.PaymentFees.App;

public static class ConfigureServices
{
    public static IServiceCollection AddPaymentFeesApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}