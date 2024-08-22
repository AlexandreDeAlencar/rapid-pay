using Microsoft.EntityFrameworkCore;
using RapidPay.CardManagement.Domain.Fees.Models;
using RapidPay.CardManagement.EntityFramework.Configurations.Fees;

namespace RapidPay.CardManagement.EntityFramework.Contexts;

public class PaymentFeesContext : DbContext
{
    private readonly Guid _instanceId = Guid.NewGuid();

    public PaymentFeesContext(DbContextOptions<PaymentFeesContext> contextOptions) : base(contextOptions)
    {
        Database.SetCommandTimeout(120);
        Console.WriteLine($"FeeManagementContext created with instanceId: {_instanceId}");
    }

    public PaymentFeesContext() { }

    public DbSet<Fee> Fees { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("public");
        builder.ApplyConfiguration(new FeesConfiguration("public"));
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(action: Console.WriteLine, minimumLevel: Microsoft.Extensions.Logging.LogLevel.Information);
    }
}
