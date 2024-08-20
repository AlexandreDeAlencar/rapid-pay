using Microsoft.EntityFrameworkCore;
using RapidPay.PaymentFees.Domain.Models;
using RapidPay.PaymentFees.EntityFramework.Configurations;

namespace RapidPay.PaymentFees.EntityFramework;

public class PayamentFeesContext : DbContext
{
    private readonly Guid _instanceId = Guid.NewGuid();

    public PayamentFeesContext(DbContextOptions<PayamentFeesContext> contextOptions) : base(contextOptions)
    {
        Database.SetCommandTimeout(120);
        Console.WriteLine($"FeeManagementContext created with instanceId: {_instanceId}");
    }

    public PayamentFeesContext() { }

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
