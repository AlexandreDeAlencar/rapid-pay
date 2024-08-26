using ErrorOr;
using Microsoft.Extensions.Caching.Memory;
using RapidPay.CardManagement.Domain.Fees.Models;
using RapidPay.CardManagement.Domain.Ports;

public class FeeRepository : IFeeRepository
{
    private readonly IMemoryCache _cache;
    private const string FeeCacheKey = "FeeCache";

    public FeeRepository(IMemoryCache cache)
    {
        _cache = cache;
    }

    public ErrorOr<Fee> GetFee()
    {
        if (_cache.TryGetValue(FeeCacheKey, out Fee? fee) && fee != null)
        {
            return fee;
        }

        return Error.Failure("Fee not found.");
    }

    public ErrorOr<Success> SaveFee(Fee fee)
    {
        // Directly store the fee in the cache
        _cache.Set(FeeCacheKey, fee);

        return new Success();
    }

    public ErrorOr<Success> UpdateFee(Fee fee)
    {
        if (!_cache.TryGetValue(FeeCacheKey, out Fee? existingFee) || existingFee == null)
        {
            return Error.Failure("Fee not found.");
        }

        // Update the existing fee
        _cache.Set(FeeCacheKey, fee);

        return new Success();
    }
}
