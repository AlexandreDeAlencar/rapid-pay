using Microsoft.Extensions.Logging;
using RapidPay.CardManagement.App.Login;

namespace RapidPay.CardManagement.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/login", async (LoginRequest request, ILoginServices loginService, ILogger logger) =>
            {
                logger.LogInformation("Login attempt for user: {Username}", request.Username);

                var result = await loginService.LoginAsync(request.Username, request.Password);

                return result.Match(
                    token =>
                    {
                        logger.LogInformation("Login successful for user: {Username}", request.Username);
                        return Results.Ok(new { token });
                    },
                    errors =>
                    {
                        logger.LogWarning("Login failed for user: {Username}. Errors: {Errors}", request.Username, errors);
                        return Results.BadRequest(errors);
                    }
                );
            })
            .WithName("Login")
            .AllowAnonymous();
        }
    }
}
