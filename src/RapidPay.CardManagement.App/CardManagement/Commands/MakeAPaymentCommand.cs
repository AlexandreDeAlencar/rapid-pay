using ErrorOr;
using MediatR;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Fees.Services;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.App.Cards.Commands
{
    public record PayWithCreditCardCommand(
        Guid CardId,
        decimal Value
        ) : IRequest<ErrorOr<Success>>;

    public class PayWithCreditCardCommandHandler : IRequestHandler<PayWithCreditCardCommand, ErrorOr<Success>>
    {
        private readonly ICardRepository _cardRepository;
        private readonly FeesExchangeService _feesExchangeService;

        public PayWithCreditCardCommandHandler(ICardRepository cardRepository, FeesExchangeService feesExchangeService)
        {
            _cardRepository = cardRepository;
            _feesExchangeService = feesExchangeService;
        }

        public async Task<ErrorOr<Success>> Handle(PayWithCreditCardCommand request, CancellationToken cancellationToken)
        {
            if (request.CardId == Guid.Empty)
            {
                return Error.Validation(description: "invalid card id");
            }

            var getCardResult = await _cardRepository.GetByIdAsync(request.CardId);

            if (getCardResult is null)
            {
                return Error.NotFound(description: "get card failed");
            }

            var currentFeeMultiplier = _feesExchangeService.GetCurrentFeeMultiplier();

            var valueWithFee = request.Value * currentFeeMultiplier;

            var newTransaction = new CardTransaction(
                Guid.NewGuid(),
                valueWithFee,
                1,
                DateTime.Now
                );

            var updateBalanceResult = getCardResult.AddTransaction(newTransaction);

            if (updateBalanceResult.IsError)
            {
                return Error.Validation(description: "update balance failed");
            }

            return new Success();
        }
    }
}
