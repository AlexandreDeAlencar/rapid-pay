using ErrorOr;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RapidPay.CardManagement.Domain.Fees.Models;

[Table("fees", Schema = "public")]
public partial class Fee
{
    #region Constructor
    private Fee(Guid feeId, decimal value, DateTime effectiveDate)
    {
        Value = value;
        EffectiveDate = effectiveDate;
    }

    private Fee() { }

    #endregion

    #region Properties
    [Key]
    [Column("feeid")]
    public Guid Id { get; private set; }

    [Column("value")]
    [Required]
    public decimal Value { get; private set; }

    [Column("effectivedate")]
    public DateTime EffectiveDate { get; private set; }
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

        var random = new Random();
        decimal newRandomMultiplier = (decimal)(random.NextDouble() * 2);
        Guid feeId = Guid.NewGuid();
        Value = (decimal)(random.NextDouble() * 2);
        EffectiveDate = DateTime.UtcNow;

        return new Success();
    }

    #endregion
}