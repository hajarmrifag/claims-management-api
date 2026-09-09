using Claims.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Claims.Infrastructure.Persistence.Configurations;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PolicyNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.PolicyNumber)
            .IsUnique();

        builder.Property(x => x.PolicyType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CoverageAmount)
            .HasPrecision(18, 2);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
