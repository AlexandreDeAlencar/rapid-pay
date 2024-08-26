using Microsoft.EntityFrameworkCore;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.EntityFramework.Configurations.CardManagement;

namespace RapidPay.CardManagement.EntityFramework.Contexts;

public class CardManagementContext : DbContext
{
    private readonly Guid _instanceId = Guid.NewGuid();

    public CardManagementContext(DbContextOptions<CardManagementContext> contextOptions) : base(contextOptions)
    {
        Database.SetCommandTimeout(120);
        Console.WriteLine($"CardManagementContext created with instanceId: {_instanceId}");
    }

    public CardManagementContext() { }

    public DbSet<Card> Cards { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("public");
        builder.ApplyConfiguration(new CardsConfiguration("public"));
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(action: Console.WriteLine, minimumLevel: Microsoft.Extensions.Logging.LogLevel.Information);
    }
}

