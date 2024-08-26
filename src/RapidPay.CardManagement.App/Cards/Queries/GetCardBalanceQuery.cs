using ErrorOr;
using MediatR;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.App.Cards.Queries
{
    public record GetCardBalanceQuery(
        Guid CardId
    ) : IRequest<ErrorOr<GetCardBalanceQueryResponse>>;

    public record GetCardBalanceQueryResponse(
        decimal Balance
    );

    public record GetCardBalanceQueryHandler(ICardRepository CardRepository) : IRequestHandler<GetCardBalanceQuery, ErrorOr<GetCardBalanceQueryResponse>>
    {
        private readonly ICardRepository _cardRepository = CardRepository;

        public async Task<ErrorOr<GetCardBalanceQueryResponse>> Handle(GetCardBalanceQuery request, CancellationToken cancellationToken)
        {
            if (request.CardId == Guid.Empty)
            {
                return Error.Validation("invalid card id");
            }

            var getCardResult = await _cardRepository.GetByIdAsync(request.CardId, false);

            if ( getCardResult is null )
            {
                return Error.NotFound(description: "unable to find card by id");
            }

            return new GetCardBalanceQueryResponse(getCardResult.Balance);
        }
    }

}
