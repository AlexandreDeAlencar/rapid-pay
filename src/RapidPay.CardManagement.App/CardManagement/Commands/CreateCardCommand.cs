using ErrorOr;
using MediatR;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.App.Cards.Commands
{
    public record CreateCardCommand(
        string UserName,
        string UserId
        ) :
        IRequest<ErrorOr<Created>>;

    public record CreateCardCommandRequest(
        string Username
        );

    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, ErrorOr<Created>>
    {
        private readonly ICardRepository _cardRepository;

        public CreateCardCommandHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<ErrorOr<Created>> Handle(CreateCardCommand request, CancellationToken cancellationToken)
        {
            // Set expiration date to 3 years from now
            var expirationDate = DateTime.Now.AddYears(3);

            var card = Card.Create(
                Guid.NewGuid(),
                GenerateRandomCreditCardNumber(),
                0, // Initial balance
                DateTime.Now, // Created at
                DateTime.Now, // Updated at
                request.UserName,
                request.UserId,
                expirationDate // Expiration date
            );

            if (card.IsError)
            {
                return Error.Validation(description: "Unable to create a new card");
            }

            await _cardRepository.AddAsync(card.Value);

            return new Created();
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
