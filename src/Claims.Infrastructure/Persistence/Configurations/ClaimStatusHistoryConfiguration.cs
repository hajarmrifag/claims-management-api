using Claims.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Claims.Infrastructure.Persistence.Configurations;

public class ClaimStatusHistoryConfiguration : IEntityTypeConfiguration<ClaimStatusHistory>
{
    public void Configure(EntityTypeBuilder<ClaimStatusHistory> builder)
    {
        builder.ToTable("ClaimStatusHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PreviousStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.NewStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne<Claim>()
            .WithMany()
            .HasForeignKey(x => x.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.ClaimId,
            x.ChangedAt
        });
    }
}
