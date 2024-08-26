using ErrorOr;
using MediatR;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.App.Cards.Commands
{
    public record CreateCardCommand(
        string UserName,
        string UserId
        ) : IRequest<ErrorOr<Guid>>;

    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, ErrorOr<Guid>>
    {
        private readonly ICardRepository _cardRepository;

        public CreateCardCommandHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateCardCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserName))
            {
                return Error.Validation(description: "Unable to create a new card");
            }

            if (string.IsNullOrEmpty(request.UserId))
            {
                return Error.Validation(description: "Invalid userId");
            }

            // Set expiration date to 3 years from now
            var expirationDate = DateTime.UtcNow.AddYears(3);

            var cardId = Guid.NewGuid();

            var card = Card.Create(
                cardId,
                GenerateRandomCreditCardNumber(),
                0, // Initial balance
                DateTime.UtcNow, // Created at
                DateTime.UtcNow, // Updated at
                request.UserName,
                request.UserId,
                expirationDate // Expiration date
            );

            if (card.IsError)
            {
                return Error.Validation(description: "Unable to create a new card");
            }

            await _cardRepository.AddAsync(card.Value);
            await _cardRepository.SaveChangesAsync(cancellationToken);

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
