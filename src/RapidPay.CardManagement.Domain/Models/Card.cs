using ErrorOr;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RapidPay.CardManagement.Domain.Models;

[Table("cards", Schema = "public")]
public partial class Card
{
    #region Constructor
    private Card(Guid cardId, string cardNumber, decimal balance, DateTime createdDate, DateTime lastUpdatedDate)
    {
        CardId = cardId;
        CardNumber = cardNumber;
        Balance = balance;
        CreatedDate = createdDate;
        LastUpdatedDate = lastUpdatedDate;
        Transactions = new List<Transaction>();
    }

    private Card() { }
    #endregion

    #region Properties
    [Key]
    [Column("cardid")]
    public Guid CardId { get; private set; }

    [Column("cardnumber")]
    [Required]
    public string CardNumber { get; private set; }

    [Column("balance")]
    [Required]
    public decimal Balance { get; private set; }

    [Column("createddate")]
    public DateTime CreatedDate { get; private set; }

    [Column("lastupdateddate")]
    public DateTime LastUpdatedDate { get; private set; }

    public ICollection<Transaction> Transactions { get; private set; }
    #endregion

    #region Static Factory
    public static ErrorOr<Card> Create(Guid cardId, string cardNumber, decimal balance, DateTime createdDate, DateTime lastUpdatedDate)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return Error.Failure("Card number is required.");
        }

        if (balance < 0)
        {
            return Error.Failure("Balance cannot be negative.");
        }

        var card = new Card(cardId, cardNumber, balance, createdDate, lastUpdatedDate);

        if (card == null)
        {
            return Error.Failure("Unable to create card.");
        }

        return card;
    }
    #endregion

    #region Public Methods
    public ErrorOr<Success> UpdateBalance(decimal newBalance)
    {
        if (newBalance < 0)
        {
            return Error.Failure("Balance cannot be negative.");
        }

        Balance = newBalance;
        LastUpdatedDate = DateTime.UtcNow;

        return new Success();
    }

    public ErrorOr<Success> AddTransaction(Transaction transaction)
    {
        if (transaction == null)
        {
            return Error.Failure("Transaction cannot be null.");
        }

        Transactions.Add(transaction);
        UpdateBalance(Balance - transaction.Amount);

        return new Success();
    }
    #endregion
}