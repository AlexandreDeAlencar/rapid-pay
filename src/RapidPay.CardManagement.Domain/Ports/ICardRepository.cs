using RapidPay.CardManagement.Domain.Cards.Models;

namespace RapidPay.CardManagement.Domain.Ports
{
    public interface ICardRepository
    {
        Task<Card?> GetByIdAsync(Guid id, bool tracking = true);
        Task<List<Card>?> GetByUserIdAsync(Guid userId, bool tracking = true);
        Task AddAsync(Card card);
        Task UpdateAsync(Card card);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
