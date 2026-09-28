using Domain.Entities.Workout.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Workout.Session;

public class WorkoutSetConfiguration : IEntityTypeConfiguration<WorkoutSet>
{
    public void Configure(EntityTypeBuilder<WorkoutSet> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkoutSessionExerciseId)
            .IsRequired();

        builder.Property(x => x.SetNumber)
            .IsRequired();

        builder.Property(x => x.Reps)
            .IsRequired();

        builder.Property(x => x.Weight)
            .HasPrecision(10, 2);

        builder.HasIndex(x => new
            {
                x.WorkoutSessionExerciseId,
                x.SetNumber
            })
            .IsUnique();

        builder.HasOne(x => x.WorkoutSessionExercise)
            .WithMany(x => x.Sets)
            .HasForeignKey(x => x.WorkoutSessionExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}