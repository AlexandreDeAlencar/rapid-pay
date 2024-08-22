using RapidPay.CardManagement.Domain.Fees.Models;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.Domain.Fees.Services
{
    public class FeeDomainService
    {
        private readonly Random _random;
        private Fee _currentFee;
        private readonly IFeeRepository _feeRepository;

        public FeeDomainService(IFeeRepository feeRepository)
        {
            _random = new Random();
            _feeRepository = feeRepository;
            _currentFee = Fee.Create(Guid.NewGuid(), 1.0m, DateTime.UtcNow).Value;
            UpdateFee().Wait();
        }

        public Fee GetCurrentFee()
        {
            return _currentFee;
        }

        public async Task<Fee> GenerateNewFeeAsync()
        {
            decimal feeRate = (decimal)(_random.NextDouble() * 2);

            var newFeeResult = Fee.Create(Guid.NewGuid(), feeRate, DateTime.UtcNow);

            if (newFeeResult.IsError)
            {
                throw new Exception("Failed to create a new fee");
            }

            var newFee = newFeeResult.Value;

            await _feeRepository.AddAsync(newFee);

            _currentFee = newFee;

            return _currentFee;
        }

        private async Task UpdateFee()
        {
            await GenerateNewFeeAsync();
        }
    }
}
