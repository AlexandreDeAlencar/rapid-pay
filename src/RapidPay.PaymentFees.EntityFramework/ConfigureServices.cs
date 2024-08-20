using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RapidPay.PaymentFees.EntityFramework;

public static class ConfigureServices
{
    public static IServiceCollection AddPaymentFeesEntityFramework(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddDbContext<PayamentFeesContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}