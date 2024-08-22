using ErrorOr;
using MediatR;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.App.Cards.Commands
{
    public record PayWithCreditCardCommand(
        Guid CardId,
        decimal Value
        ) : IRequest<ErrorOr<Success>>;

    public record PayWithCreditCardCommandHandler(ICardRepository CardRepository) : IRequestHandler<PayWithCreditCardCommand, ErrorOr<Success>>
    {
        private readonly ICardRepository _cardRepository = CardRepository;

        public async Task<ErrorOr<Success>> Handle(PayWithCreditCardCommand request, CancellationToken cancellationToken)
        {
            var getCardResult = await _cardRepository.GetByIdAsync(request.CardId);

            if (getCardResult is null)
            {
                return Error.NotFound(description: "get card failed");
            }

            var newTransaction = new CardTransaction(
                Guid.NewGuid(),
                request.Value,
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
