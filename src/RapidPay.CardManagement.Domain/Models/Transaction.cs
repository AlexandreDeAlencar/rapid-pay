using ErrorOr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RapidPay.CardManagement.Domain.Models;

[ComplexType]
public class Transaction
{
    #region Constructor
    private Transaction(Guid transactionId, decimal amount, decimal feeApplied, DateTime transactionDate, string description)
    {
        TransactionId = transactionId;
        Amount = amount;
        FeeApplied = feeApplied;
        TransactionDate = transactionDate;
        Description = description;
    }

    private Transaction() { }
    #endregion

    #region Properties
    [Column("transactionid")]
    public Guid TransactionId { get; private set; }

    [Column("amount")]
    [Required]
    public decimal Amount { get; private set; }

    [Column("feeapplied")]
    [Required]
    public decimal FeeApplied { get; private set; }

    [Column("transactiondate")]
    public DateTime TransactionDate { get; private set; }

    [Column("description")]
    public string Description { get; private set; }
    #endregion

    #region Static Factory
    public static ErrorOr<Transaction> Create(Guid transactionId, decimal amount, decimal feeApplied, DateTime transactionDate, string description)
    {
        if (amount <= 0)
        {
            return Error.Failure("Amount must be greater than zero.");
        }

        if (feeApplied < 0)
        {
            return Error.Failure("Fee applied cannot be negative.");
        }

        var transaction = new Transaction(transactionId, amount, feeApplied, transactionDate, description);

        if (transaction == null)
        {
            return Error.Failure("Unable to create transaction.");
        }

        return transaction;
    }
    #endregion

    #region Public Methods
    public ErrorOr<Success> Update(decimal amount, decimal feeApplied, DateTime transactionDate, string description)
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
        Description = description;

        return new Success();
    }
    #endregion
}