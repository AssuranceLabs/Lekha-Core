using LekhaCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LekhaCore.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
        builder.HasIndex(x => x.TokenHash).IsUnique();

        builder.Property(x => x.ReplacedByTokenHash).HasMaxLength(128);
        builder.Property(x => x.CreatedByIp).HasMaxLength(64);
        builder.Property(x => x.Created).IsRequired();
        builder.Property(x => x.Expires).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.IsRevoked });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.GUID);
    }
}
