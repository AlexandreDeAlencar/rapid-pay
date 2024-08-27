using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RapidPay.CardManagement.Domain.Ports;
using RapidPay.CardManagement.EntityFramewok.Repositories;
using RapidPay.CardManagement.EntityFramework.Contexts;

namespace RapidPay.CardManagement.EntityFramewok;

public static class ConfigureServices
{
    public static IServiceCollection AddEntityFrameworkConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCardManagementEntityFramework(configuration)
                .AddUserAuthenticationEntityFramework(configuration);

        services.AddMemoryCache();
        services.AddScoped<IFeeRepository, FeeRepository>();

        return services;
    }

    public static IServiceCollection AddCardManagementEntityFramework(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddDbContext<CardManagementContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICardRepository, CardRepository>();

        return services;
    }

    public static IServiceCollection AddUserAuthenticationEntityFramework(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddDbContext<UserAuthContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<UserAuthContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}