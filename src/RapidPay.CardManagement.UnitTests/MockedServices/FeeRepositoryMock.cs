using ErrorOr;
using RapidPay.CardManagement.Domain.Fees.Models;
using RapidPay.CardManagement.Domain.Ports;

public class FakeFeeRepository : IFeeRepository
{
    private Fee _currentFee;

    public FakeFeeRepository()
    {
        _currentFee = Fee.Create(Guid.NewGuid(), 1.0m, DateTime.UtcNow).Value;
    }

    public ErrorOr<Fee> GetFee()
    {
        return _currentFee;
    }

    public ErrorOr<Success> UpsertFee(Fee fee)
    {
        _currentFee = fee;
        return new Success();
    }
}

public class FakeFeeRepositoryWithError : IFeeRepository
{
    public ErrorOr<Fee> GetFee()
    {
        return Error.Failure("Simulated error retrieving fee");
    }

    public ErrorOr<Success> UpsertFee(Fee fee)
    {
        return Error.Failure("Simulated error upserting fee");
    }
}
