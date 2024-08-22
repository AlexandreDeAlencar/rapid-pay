using ErrorOr;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RapidPay.CardManagement.Domain.Cards.Models
{
    [Table("cards", Schema = "public")]
    public partial class Card
    {
        #region Constructor
        private Card(Guid cardId, string cardNumber, decimal balance, DateTime createdDate, DateTime lastUpdatedDate, string userName, string userId, DateTime expirationDate)
        {
            CardId = cardId;
            CardNumber = cardNumber;
            Balance = balance;
            CreatedDate = createdDate;
            LastUpdatedDate = lastUpdatedDate;
            UserName = userName;
            UserId = userId;
            ExpirationDate = expirationDate;
            Transactions = new List<CardTransaction>();
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

        [Column("username")]
        [Required]
        public string UserName { get; private set; }

        [Column("userid")]
        [Required]
        public string UserId { get; private set; }

        [Column("expirationdate")]
        [Required]
        public DateTime ExpirationDate { get; private set; }

        public ICollection<CardTransaction> Transactions { get; private set; }
        #endregion

        #region Static Factory
        public static ErrorOr<Card> Create(Guid cardId, string cardNumber, decimal balance, DateTime createdDate, DateTime lastUpdatedDate, string userName, string userId, DateTime expirationDate)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                return Error.Failure("Card number is required.");
            }

            if (balance < 0)
            {
                return Error.Failure("Balance cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                return Error.Failure("User name is required.");
            }

            if (expirationDate <= DateTime.UtcNow)
            {
                return Error.Failure("Expiration date must be in the future.");
            }

            var card = new Card(cardId, cardNumber, balance, createdDate, lastUpdatedDate, userName, userId, expirationDate);

            if (card == null)
            {
                return Error.Failure("Unable to create card.");
            }

            return card;
        }
        #endregion

        #region Public Methods
        private ErrorOr<Success> UpdateBalance(decimal newBalance)
        {
            if (newBalance < 0)
            {
                return Error.Failure("Balance cannot be negative.");
            }

            Balance = newBalance;
            LastUpdatedDate = DateTime.UtcNow;

            return new Success();
        }

        public ErrorOr<Success> AddTransaction(CardTransaction transaction)
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
}
