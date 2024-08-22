using Microsoft.EntityFrameworkCore;
using RapidPay.CardManagement.Domain.Fees.Models;
using RapidPay.CardManagement.Domain.Ports;
using RapidPay.CardManagement.EntityFramework.Contexts;

namespace RapidPay.CardManagement.EntityFramework.Repositories
{
    public class FeeRepository : IFeeRepository
    {
        private readonly PaymentFeesContext _context;

        public FeeRepository(PaymentFeesContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Fee fee)
        {
            await _context.Fees.AddAsync(fee);
            await _context.SaveChangesAsync();
        }

        public async Task<Fee?> GetLatestAsync()
        {
            return await _context.Fees
                .OrderByDescending(f => f.Effectivedate)
                .FirstOrDefaultAsync();
        }
    }
}
