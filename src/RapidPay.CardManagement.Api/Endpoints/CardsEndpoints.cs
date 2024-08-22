using MediatR;
using RapidPay.CardManagement.App.Cards.Commands;
using RapidPay.CardManagement.App.Cards.Queries;

namespace RapidPay.CardManagement.Api.Endpoints
{
    public static class CardsEndpoints
    {
        public static void MapCardsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/cards", async (HttpContext httpContext, IMediator mediator) =>
            {
                // Extract UserName and UserId from the token claims
                var userName = httpContext.User.Identity?.Name;
                var userId = httpContext.User.FindFirst("id")?.Value;

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId))
                {
                    return Results.BadRequest("Invalid token claims");
                }

                var command = new CreateCardCommand(userName, userId);
                var result = await mediator.Send(command);

                return result.Match(
                    created => Results.Created($"/api/cards/{userId}", created),
                    errors => Results.BadRequest(errors)
                );
            })
            .RequireAuthorization();

            app.MapPost("/api/cards/pay", async (PayWithCreditCardCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);

                return result.Match(
                    success => Results.Ok(success),
                    errors => Results.BadRequest(errors)
                );
            })
            .RequireAuthorization();

            app.MapGet("/api/cards/{cardId}/balance", async (Guid cardId, IMediator mediator) =>
            {
                var query = new GetCardBalanceQuery(cardId);
                var result = await mediator.Send(query);

                return result.Match(
                    balance => Results.Ok(balance),
                    errors => Results.BadRequest(errors)
                );
            })
            .RequireAuthorization();
        }
    }   
}
