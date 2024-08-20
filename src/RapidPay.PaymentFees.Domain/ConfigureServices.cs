using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RapidPay.PaymentFees.Domain;

public static class ConfigureServices
{
    public static IServiceCollection AddPaymentFeesDomain(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}