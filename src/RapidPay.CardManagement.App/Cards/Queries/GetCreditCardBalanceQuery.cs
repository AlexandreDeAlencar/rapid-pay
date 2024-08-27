using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.App.Cards.Queries
{
    public record GetCreditCardBalanceQuery(
        Guid CardId
    ) : IRequest<ErrorOr<GetCardBalanceQueryResponse>>;

    public record GetCardBalanceQueryResponse(
        decimal Balance
    );

    public class GetCardBalanceQueryValidator : AbstractValidator<GetCreditCardBalanceQuery>
    {
        public GetCardBalanceQueryValidator()
        {
            RuleFor(x => x.CardId)
                .NotEmpty().WithMessage("Card ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Card ID cannot be an empty GUID.");
        }
    }

    public class GetCardBalanceQueryHandler : IRequestHandler<GetCreditCardBalanceQuery, ErrorOr<GetCardBalanceQueryResponse>>
    {
        private readonly ICardRepository _cardRepository;
        private readonly ILogger<GetCardBalanceQueryHandler> _logger;

        public GetCardBalanceQueryHandler(ICardRepository cardRepository, ILogger<GetCardBalanceQueryHandler> logger)
        {
            _cardRepository = cardRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<GetCardBalanceQueryResponse>> Handle(GetCreditCardBalanceQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving balance for card: {CardId}", request.CardId);

            var getCardResult = await _cardRepository.GetByIdAsync(request.CardId, false);

            if (getCardResult is null)
            {
                _logger.LogWarning("Failed to retrieve balance: Card not found. CardId: {CardId}", request.CardId);
                return Error.NotFound(description: "Unable to find card by ID");
            }

            _logger.LogInformation("Successfully retrieved balance for card: {CardId}. Balance: {Balance}", request.CardId, getCardResult.Balance);

            return new GetCardBalanceQueryResponse(getCardResult.Balance);
        }
    }
}
