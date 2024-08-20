using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RapidPay.CardManagement.EntityFramewok;

public static class ConfigureServices
{
    public static IServiceCollection AddCardManagementEntityFramework(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddDbContext<CardManagementContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}