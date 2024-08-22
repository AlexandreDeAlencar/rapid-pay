using Microsoft.Extensions.DependencyInjection;
using RapidPay.CardManagement.Domain.Fees.Services;
using System.Reflection;

namespace RapidPay.CardManagement.Domain;

public static class ConfigureServices
{
    public static IServiceCollection AddCardManagementDomain(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddSingleton<FeeDomainService>();

        return services;
    }
}