using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .IsRequired();

        builder.Property(r => r.UserId);
        builder.HasIndex(r => r.UserId);

        builder.Property(r => r.Value)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.ExpiresAt);
        builder.Property(r => r.RevokedAt);

        builder.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}