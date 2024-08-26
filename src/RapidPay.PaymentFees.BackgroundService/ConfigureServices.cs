using Microsoft.Extensions.DependencyInjection;

namespace RapidPay.PaymentFees.BackgroundService;

public static class ServiceRegistration
{
    public static IServiceCollection AddPaymentFeesBackgroundService(this IServiceCollection services)
    {
        services.AddHostedService<FeeUpdateHostedService>();

        return services;
    }
}