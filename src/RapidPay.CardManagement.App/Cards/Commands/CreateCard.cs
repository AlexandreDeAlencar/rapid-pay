using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.App.Cards.Commands
{
    public record CreateCard(
        string UserName,
        string UserId
    ) : IRequest<ErrorOr<Guid>>;

    public class CreateCardValidator : AbstractValidator<CreateCard>
    {
        public CreateCardValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }

    public class CreateCardCommandHandler : IRequestHandler<CreateCard, ErrorOr<Guid>>
    {
        private readonly ICardRepository _cardRepository;
        private readonly ILogger<CreateCardCommandHandler> _logger;

        public CreateCardCommandHandler(ICardRepository cardRepository, ILogger<CreateCardCommandHandler> logger)
        {
            _cardRepository = cardRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateCard request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to create a card for user: {UserName} with UserId: {UserId}", request.UserName, request.UserId);

            var expirationDate = DateTime.UtcNow.AddYears(3);
            var cardId = Guid.NewGuid();

            var card = Card.Create(
                cardId,
                GenerateRandomCreditCardNumber(),
                50000,
                DateTime.UtcNow,
                DateTime.UtcNow,
                request.UserName,
                request.UserId,
                expirationDate
            );

            if (card.IsError)
            {
                _logger.LogError("Card creation failed for user: {UserName} with UserId: {UserId}. Error: {Errors}", request.UserName, request.UserId, card.Errors);
                return Error.Validation(description: "Unable to create a new card");
            }

            _logger.LogInformation("Card created successfully for user: {UserName} with UserId: {UserId}. CardId: {CardId}", request.UserName, request.UserId, cardId);

            await _cardRepository.AddAsync(card.Value);
            await _cardRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Card saved successfully to the database. CardId: {CardId}", cardId);

            return cardId;
        }

        private static string GenerateRandomCreditCardNumber()
        {
            Random random = new Random();
            char[] creditCardNumber = new char[15];

            for (int i = 0; i < 15; i++)
            {
                creditCardNumber[i] = (char)('0' + random.Next(0, 10));
            }

            return new string(creditCardNumber);
        }
    }
}
