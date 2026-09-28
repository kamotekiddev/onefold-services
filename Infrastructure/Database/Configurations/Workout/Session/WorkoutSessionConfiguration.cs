using Domain.Entities.Workout.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Workout.Session;

public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
{
    public void Configure(EntityTypeBuilder<WorkoutSession> builder)
    {
        builder.HasKey(ws => ws.Id);

        builder.Property(ws => ws.UserId)
            .IsRequired();

        builder.Property(ws => ws.WorkoutTemplateId)
            .IsRequired();

        builder.Property(ws => ws.StartedAt)
            .IsRequired();

        builder.Property(ws => ws.UpdatedAt)
            .IsRequired();

        builder.HasIndex(ws => new { ws.UserId, ws.WorkoutTemplateId });

        builder.HasOne(ws => ws.User)
            .WithMany(u => u.WorkoutSessions)
            .HasForeignKey(ws => ws.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ws => ws.WorkoutTemplate)
            .WithMany(wt => wt.WorkoutSessions)
            .HasForeignKey(ws => ws.WorkoutTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}