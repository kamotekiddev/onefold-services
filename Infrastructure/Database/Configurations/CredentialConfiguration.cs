using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(c => new { c.UserId, c.Provider })
            .IsUnique();

        builder.Property(c => c.Value)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Credentials)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}