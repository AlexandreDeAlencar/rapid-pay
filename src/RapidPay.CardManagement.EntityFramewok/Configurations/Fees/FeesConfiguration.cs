using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RapidPay.CardManagement.Domain.Fees.Models;


namespace RapidPay.CardManagement.EntityFramework.Configurations.Fees;
internal class FeesConfiguration : IEntityTypeConfiguration<Fee>
{
    private readonly string _schema;

    public FeesConfiguration(string schema)
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<Fee> builder)
    {
        ConfigureFeesTable(builder);

        Console.WriteLine($"FeesConfiguration executed with schema: {_schema}");
    }

    private void ConfigureFeesTable(EntityTypeBuilder<Fee> builder)
    {
        builder.ToTable("fees", _schema);

        builder.HasKey(f => f.Id);

        builder
            .Property(f => f.Id)
            .HasColumnName("feeid")
            .IsRequired();

        builder
            .Property(f => f.Feerate)
            .HasColumnName("feerate")
            .IsRequired();

        builder
            .Property(f => f.Effectivedate)
            .HasColumnName("effectivedate")
            .IsRequired();
    }
}