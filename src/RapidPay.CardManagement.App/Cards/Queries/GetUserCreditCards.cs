using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.App.Cards.Queries;
public record GetUserCreditCardsQuery(
    Guid UserId
) : IRequest<ErrorOr<GetUserCreditCardsQueryResponse>>;

public record GetUserCreditCardsQueryResponse(
    List<CreditCardDetails> CreditCards
);

public record CreditCardDetails(
    Guid CardId,
    string CardNumber,
    decimal Balance,
    DateTime ExpirationDate
);

public class GetUserCreditCardsQueryValidator : AbstractValidator<GetUserCreditCardsQuery>
{
    public GetUserCreditCardsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}

public class GetUserCreditCardsQueryHandler : IRequestHandler<GetUserCreditCardsQuery, ErrorOr<GetUserCreditCardsQueryResponse>>
{
    private readonly ICardRepository _cardRepository;
    private readonly ILogger<GetUserCreditCardsQueryHandler> _logger;

    public GetUserCreditCardsQueryHandler(ICardRepository cardRepository, ILogger<GetUserCreditCardsQueryHandler> logger)
    {
        _cardRepository = cardRepository;
        _logger = logger;
    }

    public async Task<ErrorOr<GetUserCreditCardsQueryResponse>> Handle(GetUserCreditCardsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving credit cards for user: {UserId}", request.UserId);

        var creditCards = await _cardRepository.GetByUserIdAsync(request.UserId, false);

        if (creditCards == null || !creditCards.Any())
        {
            _logger.LogWarning("No credit cards found for user: {UserId}", request.UserId);
            return Error.NotFound(description: "No credit cards found for the specified user.");
        }

        var creditCardDetails = creditCards.Select(card => new CreditCardDetails(
            card.Id,
            card.CardNumber,
            card.Balance,
            card.ExpirationDate
        )).ToList();

        _logger.LogInformation("Successfully retrieved {Count} credit cards for user: {UserId}", creditCardDetails.Count, request.UserId);

        return new GetUserCreditCardsQueryResponse(creditCardDetails);
    }
}