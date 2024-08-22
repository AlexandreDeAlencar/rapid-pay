using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RapidPay.CardManagement.App.Common.Behaviors;
using RapidPay.CardManagement.App.Login;
using RapidPay.CardManagement.App.UserLogin;

namespace RapidPay.CardManagement.App;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddCardManagementApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddScoped<ITokenServices, TokenServices>();
        services.AddScoped<ILoginServices, LoginServices>();

        return services;
    }
}