using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RapidPay.CardManagement.Domain.Cards.Models;

namespace RapidPay.CardManagement.EntityFramework.Configurations.CardManagement;

internal class CardsConfiguration : IEntityTypeConfiguration<Card>
{
    private readonly string _schema;

    public CardsConfiguration(string schema)
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<Card> builder)
    {
        ConfigureCardsTable(builder);

        Console.WriteLine($"CardsConfiguration executed with schema: {_schema}");
    }

    private void ConfigureCardsTable(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("cards", _schema);

        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Id)
            .HasColumnName("cardid")
            .IsRequired();

        builder
            .Property(c => c.CardNumber)
            .HasColumnName("cardnumber")
            .IsRequired();

        builder
            .Property(c => c.Balance)
            .HasColumnName("balance")
            .IsRequired();

        builder
            .Property(c => c.CreatedDate)
            .HasColumnName("createddate")
            .IsRequired();

        builder
            .Property(c => c.LastUpdatedDate)
            .HasColumnName("lastupdateddate")
            .IsRequired();

        builder
            .Property(c => c.UserName)
            .HasColumnName("username")
            .IsRequired();

        builder
            .Property(c => c.UserId)
            .HasColumnName("userid")
            .IsRequired();

        builder
            .Property(c => c.ExpirationDate)
            .HasColumnName("expirationdate")
            .IsRequired();

        ConfigureTransactionsTable(builder);
    }

    private void ConfigureTransactionsTable(EntityTypeBuilder<Card> builder)
    {
        builder.OwnsMany(c => c.Transactions, transactionBuilder =>
        {
            transactionBuilder.ToTable("transactions", _schema);

            transactionBuilder
                .WithOwner()
                .HasForeignKey("CardId");

            transactionBuilder.HasKey(nameof(CardTransaction.Id), "CardId");

            transactionBuilder
                .Property("CardId")
                .HasColumnName("cardid")
                .IsRequired();

            transactionBuilder
                .Property(t => t.Id)
                .HasColumnName("transactionid")
                .ValueGeneratedOnAdd()
                .IsRequired();

            transactionBuilder
                .Property(t => t.Amount)
                .HasColumnName("amount")
                .IsRequired();

            transactionBuilder
                .Property(t => t.FeeApplied)
                .HasColumnName("feeapplied")
                .IsRequired();

            transactionBuilder
                .Property(t => t.TransactionDate)
                .HasColumnName("transactiondate")
                .IsRequired();
        });

        builder.Metadata
            .FindNavigation(nameof(Card.Transactions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}