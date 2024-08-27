using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.App.Cards.Commands
{
    public record PayWithCreditCardCommand(
        Guid CardId,
        decimal Value
    ) : IRequest<ErrorOr<Success>>;

    public class PayWithCreditCardCommandValidator : AbstractValidator<PayWithCreditCardCommand>
    {
        public PayWithCreditCardCommandValidator()
        {
            RuleFor(x => x.CardId)
                .NotEmpty().WithMessage("Card ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Card ID cannot be an empty GUID.");

            RuleFor(x => x.Value)
                .GreaterThan(0).WithMessage("Payment value must be greater than zero.");
        }
    }

    public class PayWithCreditCardCommandHandler : IRequestHandler<PayWithCreditCardCommand, ErrorOr<Success>>
    {
        private readonly ICardRepository _cardRepository;
        private readonly IFeeRepository _feeRepository;
        private readonly ILogger<PayWithCreditCardCommandHandler> _logger;

        public PayWithCreditCardCommandHandler(
            ICardRepository cardRepository,
            IFeeRepository feeRepository,
            ILogger<PayWithCreditCardCommandHandler> logger)
        {
            _cardRepository = cardRepository;
            _feeRepository = feeRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<Success>> Handle(PayWithCreditCardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing payment with card: {CardId} for value: {Value}", request.CardId, request.Value);

            var getCardResult = await _cardRepository.GetByIdAsync(request.CardId);

            if (getCardResult is null)
            {
                _logger.LogWarning("Payment failed: Card not found. CardId: {CardId}", request.CardId);
                return Error.NotFound(description: "Card not found");
            }

            var feeResult = _feeRepository.GetFee();

            if (feeResult.IsError)
            {
                _logger.LogError("Payment failed: Unable to retrieve fee for CardId: {CardId}. Errors: {Errors}", request.CardId, feeResult.Errors);
                return Error.Failure(description: "Unable to retrieve fee");
            }

            var fee = feeResult.Value;
            var valueWithFee = request.Value * fee.Value;

            _logger.LogInformation("Applying fee: {FeeValue} to the payment. Total value with fee: {ValueWithFee}", fee.Value, valueWithFee);

            var newTransaction = new CardTransaction(
                Guid.NewGuid(),
                valueWithFee,
                fee.Value,
                DateTime.UtcNow
            );

            var updateBalanceResult = getCardResult.AddTransaction(newTransaction);

            if (updateBalanceResult.IsError)
            {
                _logger.LogWarning("Payment failed: Unable to update balance for CardId: {CardId}. Errors: {Errors}", request.CardId, updateBalanceResult.Errors);
                return updateBalanceResult;
            }

            await _cardRepository.SaveChangesAsync();

            _logger.LogInformation("Payment processed successfully for CardId: {CardId}. TransactionId: {TransactionId}", request.CardId, newTransaction.Id);

            return new Success();
        }
    }
}
