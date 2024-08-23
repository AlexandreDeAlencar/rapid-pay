namespace RapidPay.CardManagement.Domain.Fees.Services
{
    public class FeesExchangeService
    {
        private decimal _currentFeeMultiplier;
        private readonly Timer _timer;

        public FeesExchangeService()
        {
            _currentFeeMultiplier = 1.0m;
            _timer = new Timer(UpdateFee, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            UpdateFee(null);
        }

        public decimal GetCurrentFeeMultiplier() => _currentFeeMultiplier;

        private void UpdateFee(object? state)
        {
            var random = new Random();
            decimal newRandomMultiplier = (decimal)(random.NextDouble() * 2);
            _currentFeeMultiplier = newRandomMultiplier;
        }
    }
}
