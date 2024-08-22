using RapidPay.CardManagement.Domain.Fees.Models;

namespace RapidPay.CardManagement.Domain.Ports
{
    public interface IFeeRepository
    {
        Task AddAsync(Fee fee);
        Task<Fee?> GetLatestAsync();
    }
}
