using MediatR;
using Microsoft.Extensions.Logging;
using RapidPay.CardManagement.App.Cards.Commands;
using RapidPay.CardManagement.App.Cards.Queries;

namespace RapidPay.CardManagement.Api.Endpoints
{
    public static class CardsEndpoints
    {
        public static void MapCardsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/cards/create", async (HttpContext httpContext, IMediator mediator, ILogger logger) =>
            {
                var userName = httpContext.User.FindFirst("name")?.Value;
                var userId = httpContext.User.FindFirst("id")?.Value;

                logger.LogInformation("CreateCard endpoint called. UserId: {UserId}, UserName: {UserName}", userId, userName);

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("CreateCard endpoint failed due to missing token claims.");
                    return Results.BadRequest("Invalid token claims");
                }

                var command = new CreateCard(userName, userId);
                var result = await mediator.Send(command);

                return result.Match(
                    created =>
                    {
                        logger.LogInformation("Card created successfully. UserId: {UserId}, CardId: {CardId}", userId, created);
                        return Results.Created($"/api/cards/{userId}", created);
                    },
                    errors =>
                    {
                        logger.LogWarning("Card creation failed for UserId: {UserId}. Errors: {Errors}", userId, errors);
                        return Results.BadRequest(errors);
                    }
                );
            })
            .RequireAuthorization();

            app.MapPost("/api/cards/payment", async (PayWithCreditCardCommand command, IMediator mediator, ILogger logger) =>
            {
                logger.LogInformation("PayWithCreditCard endpoint called. CardId: {CardId}, Value: {Value}", command.CardId, command.Value);

                var result = await mediator.Send(command);

                return result.Match(
                    success =>
                    {
                        logger.LogInformation("Payment processed successfully for CardId: {CardId}", command.CardId);
                        return Results.Ok(success);
                    },
                    errors =>
                    {
                        logger.LogWarning("Payment failed for CardId: {CardId}. Errors: {Errors}", command.CardId, errors);
                        return Results.BadRequest(errors);
                    }
                );
            })
            .RequireAuthorization();

            app.MapGet("/api/cards/{cardId}/balance", async (Guid cardId, IMediator mediator, ILogger logger) =>
            {
                logger.LogInformation("GetCardBalance endpoint called. CardId: {CardId}", cardId);

                var query = new GetCardBalanceQuery(cardId);
                var result = await mediator.Send(query);

                return result.Match(
                    balance =>
                    {
                        logger.LogInformation("Balance retrieved successfully for CardId: {CardId}. Balance: {Balance}", cardId, balance.Balance);
                        return Results.Ok(balance);
                    },
                    errors =>
                    {
                        logger.LogWarning("Failed to retrieve balance for CardId: {CardId}. Errors: {Errors}", cardId, errors);
                        return Results.BadRequest(errors);
                    }
                );
            })
            .RequireAuthorization();
        }
    }
}
