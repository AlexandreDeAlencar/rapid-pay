using Microsoft.EntityFrameworkCore;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Ports;
using RapidPay.CardManagement.EntityFramework.Contexts;

namespace RapidPay.CardManagement.EntityFramewok.Repositories
{
    public class CardRepository(CardManagementContext cardManagementContext) : ICardRepository
    {
        private readonly CardManagementContext _cardManagementContext = cardManagementContext;

        public async Task AddAsync(Card card)
        {
            await _cardManagementContext.Cards.AddAsync(card);
        }

        public async Task<Card?> GetByIdAsync(Guid id, bool tracking = true)
        {
            var cardQuery = _cardManagementContext.Cards;

            if (tracking is false)
            {
                cardQuery.AsNoTracking();
            }

            return await cardQuery.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Card>?> GetByUserIdAsync(Guid userId, bool tracking = true)
        {
            if (_cardManagementContext?.Cards == null)
            {
                return new();
            }

            var cardQuery = _cardManagementContext.Cards.AsQueryable();

            if (!tracking)
            {
                cardQuery = cardQuery.AsNoTracking();
            }

            return await cardQuery
                .Where(c => c.UserId == userId.ToString())
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _cardManagementContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Card card)
        {
            _cardManagementContext.Cards.Update(card);
            await _cardManagementContext.SaveChangesAsync();
        }
    }
}
