using ErrorOr;
using Microsoft.Extensions.Caching.Memory;
using RapidPay.CardManagement.Domain.Fees.Models;

public class FeesExchangeService
{
    private readonly IMemoryCache _cache;
    private readonly Timer _timer;

    public FeesExchangeService(IMemoryCache cache)
    {
        _cache = cache;
        _timer = new Timer(UpdateFee, null, TimeSpan.Zero, TimeSpan.FromHours(1));
    }

    public ErrorOr<Fee> GetCurrentFee()
    {
        // Try to get the fee from the cache
        if (_cache.TryGetValue("CurrentFee", out Fee fee))
        {
            return fee;
        }

        return Error.Failure("Fee not found in cache.");
    }

    private void UpdateFee(object? state)
    {
        var random = new Random();
        decimal newRandomMultiplier = (decimal)(random.NextDouble() * 2);
        Guid feeId = Guid.NewGuid();
        DateTime effectiveDate = DateTime.UtcNow;

        var feeResult = Fee.Create(feeId, newRandomMultiplier, effectiveDate);

        if (feeResult.IsError)
        {
            return;
        }

        Fee fee = feeResult.Value;

        _cache.Set("CurrentFee", fee, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        });
    }
}
