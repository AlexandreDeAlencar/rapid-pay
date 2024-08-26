using ErrorOr;
using RapidPay.CardManagement.Domain.Fees.Models;

namespace RapidPay.CardManagement.Domain.Ports;

public interface IFeeRepository
{
    ErrorOr<Fee> GetFee();
    ErrorOr<Success> UpsertFee(Fee fee);
}
