using Microsoft.Extensions.Logging;
using RapidPay.CardManagement.App.Login;

namespace RapidPay.CardManagement.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/login", async (LoginRequest request, ILoginServices loginService) =>
        {
            var result = await loginService.LoginAsync(request.Username, request.Password);

            return result.Match(
                token =>
                {
                    return Results.Ok(new { token });
                },
                errors =>
                {
                    return Results.BadRequest(errors);
                }
            );
        })
        .WithName("Login")
        .AllowAnonymous();
    }
}
