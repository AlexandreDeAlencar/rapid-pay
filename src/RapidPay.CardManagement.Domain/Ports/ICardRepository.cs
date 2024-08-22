using RapidPay.CardManagement.Domain.Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidPay.CardManagement.Domain.Ports
{
    public interface ICardRepository
    {
        Task<Card?> GetByIdAsync(Guid id, bool tracking = true);
        Task AddAsync(Card card);
        Task UpdateAsync(Card card);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
