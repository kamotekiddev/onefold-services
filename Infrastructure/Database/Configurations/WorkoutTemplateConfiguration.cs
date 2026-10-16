using Domain.Entities.Workout.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class WorkoutTemplateConfiguration : IEntityTypeConfiguration<WorkoutTemplate>
{
    public void Configure(EntityTypeBuilder<WorkoutTemplate> builder)
    {
        builder.HasKey(wt => wt.Id);

        builder.Ignore(wt => wt.ScheduledDays);
        builder.Property<DayOfWeek[]>("_scheduledDays")
            .HasColumnName("ScheduledDays")
            .HasColumnType("integer[]")
            .HasDefaultValueSql("'{}'::integer[]")
            .IsRequired();

        builder.HasOne(wt => wt.User)
            .WithMany(u => u.WorkoutTemplates)
            .HasForeignKey(wt => wt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(wt => new { wt.Name, wt.UserId })
            .IsUnique();
    }
}
