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

    public class PayWithCreditCardCommandHandler : IRequestHandler<PayWithCreditCardCommand, ErrorOr<Success>>
    {
        private readonly ICardRepository _cardRepository;
        private readonly IFeeRepository _feeRepository;

        public PayWithCreditCardCommandHandler(ICardRepository cardRepository, IFeeRepository feeRepository)
        {
            _cardRepository = cardRepository;
            _feeRepository = feeRepository;
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

            var feeResult = _feeRepository.GetFee();

            if (feeResult.IsError)
            {
                return Error.Failure(description: "unable to get fee");
            }

            var fee = feeResult.Value;

            var valueWithFee = request.Value * fee.Value;

            var newTransaction = new CardTransaction(
                Guid.NewGuid(),
                valueWithFee,
                fee.Value,
                DateTime.UtcNow
                );

            var updateBalanceResult = getCardResult.AddTransaction(newTransaction);

            if (updateBalanceResult.IsError)
            {
                return Error.Validation(description: "update balance failed");
            }

            await _cardRepository.SaveChangesAsync();

            return new Success();
        }
    }
}
