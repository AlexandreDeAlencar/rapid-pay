using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Ports;

public class FakeCardRepository : ICardRepository
{
    // Store the cards in-memory for testing
    private readonly List<Card> _cards = new List<Card>();

    public List<Card> AddedCards => _cards;

    public Task AddAsync(Card card)
    {
        _cards.Add(card);
        return Task.CompletedTask;
    }

    public Task<Card?> GetByIdAsync(Guid id)
    {
        var createdCard = Card.Create(
            id,
            "123456789012345",
            0,
            DateTime.Now,
            DateTime.Now,
            "MockUser",
            "MockUserId",
            DateTime.Now.AddYears(3)
        );

        if (createdCard.IsError)
        {
            return Task.FromResult<Card?>(null);
        }

        return Task.FromResult(createdCard.Value);
    }

    public Task<Card?> GetByIdAsync(Guid id, bool tracking = true)
    {
        return GetByIdAsync(id);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(1);
    }

    public Task UpdateAsync(Card card)
    {
        return Task.CompletedTask;
    }

    public class FakeCardRepositoryWithError : ICardRepository
    {
        public Task AddAsync(Card card)
        {
            throw new InvalidOperationException("Simulated error during card creation");
        }

        public Task<Card?> GetByIdAsync(Guid id)
        {
            return Task.FromResult<Card?>(null);
        }

        public Task<Card?> GetByIdAsync(Guid id, bool tracking = true)
        {
            return Task.FromResult<Card?>(null);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Simulated error during save changes");
        }

        public Task UpdateAsync(Card card)
        {
            throw new InvalidOperationException("Simulated error during update");
        }
    }
}
