using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Ports;

public class FakeCardRepository : ICardRepository
{
    private readonly List<Card> _cards = new List<Card>();

    public List<Card> AddedCards => _cards;

    public Task AddAsync(Card card)
    {
        _cards.Add(card);
        return Task.CompletedTask;
    }

    public Task<Card?> GetByIdAsync(Guid id)
    {
        var card = _cards.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(card);
    }

    public Task<Card?> GetByIdAsync(Guid id, bool tracking = true)
    {
        return GetByIdAsync(id);
    }

    public Task<List<Card>?> GetByUserIdAsync(Guid userId, bool tracking = true)
    {
        var cards = _cards.Where(c => c.UserId == userId.ToString()).ToList();
        return Task.FromResult(cards.Any() ? cards : null);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(1);
    }

    public Task UpdateAsync(Card card)
    {
        var existingCard = _cards.FirstOrDefault(c => c.Id == card.Id);
        if (existingCard != null)
        {
            _cards.Remove(existingCard);
            _cards.Add(card);
        }
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

        public Task<List<Card>?> GetByUserIdAsync(Guid userId, bool tracking = true)
        {
            throw new InvalidOperationException("Simulated error during retrieval by user ID");
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
