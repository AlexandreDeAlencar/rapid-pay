using MediatR;
using RapidPay.CardManagement.App.Cards.Commands;
using RapidPay.CardManagement.App.Cards.Queries;

namespace RapidPay.CardManagement.Api.Endpoints;

public static class CardsEndpoints
{
    public static void MapCardsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/cards/create", async (HttpContext httpContext, IMediator mediator) =>
        {
            var userName = httpContext.User.FindFirst("name")?.Value;
            var userId = httpContext.User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId))
            {
                return Results.BadRequest("Invalid token claims");
            }

            var command = new CreateCard(userName, userId);
            var result = await mediator.Send(command);

            return result.Match(
                created =>
                {
                    return Results.Created($"/api/cards/{userId}", created);
                },
                errors =>
                {
                    return Results.BadRequest(errors);
                }
            );
        })
        .RequireAuthorization();

        app.MapPost("/api/cards/payment", async (PayWithCreditCardCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);

            return result.Match(
                success =>
                {
                    return Results.Ok(success);
                },
                errors =>
                {
                    return Results.BadRequest(errors);
                }
            );
        })
        .RequireAuthorization();

        app.MapGet("/api/cards/{cardId}/balance", async (Guid cardId, IMediator mediator) =>
        {
            var query = new GetCreditCardBalanceQuery(cardId);
            var result = await mediator.Send(query);

            return result.Match(
                balance =>
                {
                    return Results.Ok(balance);
                },
                errors =>
                {
                    return Results.BadRequest(errors);
                }
            );
        })
        .RequireAuthorization();

        app.MapGet("/api/cards", async (HttpContext httpContext, IMediator mediator) =>
        {
            var userName = httpContext.User.FindFirst("name")?.Value;
            var userIdString = httpContext.User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                return Results.BadRequest("User name is missing from the token claims.");
            }

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Results.BadRequest("Invalid or missing User ID in the token claims.");
            }

            var query = new GetUserCreditCardsQuery(userId);
            var result = await mediator.Send(query);

            return result.Match(
                cards =>
                {
                    return Results.Ok(cards);
                },
                errors =>
                {
                    return Results.BadRequest(errors);
                }
            );
        })
        .RequireAuthorization();
    }
}
