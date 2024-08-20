using ErrorOr;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RapidPay.PaymentFees.Domain.Models;

[Table("fees", Schema = "public")]
public partial class Fee
{
    #region Constructor
    private Fee(Guid feeId, decimal feeRate, DateTime effectiveDate)
    {
        Feerate = feeRate;
        Effectivedate = effectiveDate;
    }

    private Fee() { }

    #endregion

    #region Properties
    [Key]
    [Column("feeid")]
    public Guid Id { get; private set; }

    [Column("feerate")]
    [Required]
    public decimal Feerate { get; private set; }

    [Column("effectivedate")]
    public DateTime Effectivedate { get; private set; }
    #endregion

    #region Static Factory
    public static ErrorOr<Fee> Create(Guid feeId, decimal feeRate, DateTime effectiveDate)
    {
        if (feeRate <= 0)
        {
            return Error.Failure("Fee rate must be greater than zero.");
        }

        var fee = new Fee(feeId, feeRate, effectiveDate);

        if (fee == null)
        {
            return Error.Failure("Unable to create fee.");
        }

        return fee;
    }
    #endregion

    #region Public Methods
    public ErrorOr<Success> Update(decimal feeRate, DateTime effectiveDate)
    {
        if (feeRate <= 0)
        {
            return Error.Failure("Fee rate must be greater than zero.");
        }

        Feerate = feeRate;
        Effectivedate = effectiveDate;

        return new Success();
    }

    #endregion
}