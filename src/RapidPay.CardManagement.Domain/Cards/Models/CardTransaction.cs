using ErrorOr;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RapidPay.CardManagement.Domain.Cards.Models;

[ComplexType]
public class CardTransaction
{
    #region Constructor
    public CardTransaction(Guid transactionId, decimal amount, decimal feeApplied, DateTime transactionDate)
    {
        Id = transactionId;
        Amount = amount;
        FeeApplied = feeApplied;
        TransactionDate = transactionDate;
    }

    private CardTransaction() { }
    #endregion

    #region Properties
    [Column("transactionid")]
    public Guid Id { get; private set; }

    [Column("amount")]
    [Required]
    public decimal Amount { get; private set; }

    [Column("feeapplied")]
    [Required]
    public decimal FeeApplied { get; private set; }

    [Column("transactiondate")]
    public DateTime TransactionDate { get; private set; }
    #endregion

    #region Static Factory
    public static ErrorOr<CardTransaction> Create(Guid transactionId, decimal amount, decimal feeApplied, DateTime transactionDate)
    {
        if (amount <= 0)
        {
            return Error.Failure("Amount must be greater than zero.");
        }

        if (feeApplied < 0)
        {
            return Error.Failure("Fee applied cannot be negative.");
        }

        var transaction = new CardTransaction(transactionId, amount, feeApplied, transactionDate);

        if (transaction == null)
        {
            return Error.Failure("Unable to create transaction.");
        }

        return transaction;
    }
    #endregion

    #region Public Methods
    public ErrorOr<Success> Update(decimal amount, decimal feeApplied, DateTime transactionDate)
    {
        if (amount <= 0)
        {
            return Error.Failure("Amount must be greater than zero.");
        }

        if (feeApplied < 0)
        {
            return Error.Failure("Fee applied cannot be negative.");
        }

        Amount = amount;
        FeeApplied = feeApplied;
        TransactionDate = transactionDate;

        return new Success();
    }
    #endregion
}