using Domain.Entities.Workout.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Workout.Session;

public class WorkoutSessionExerciseConfiguration : IEntityTypeConfiguration<WorkoutSessionExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutSessionExercise> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SessionId)
            .IsRequired();

        builder.Property(x => x.ExerciseId)
            .IsRequired();

        builder.Property(x => x.TargetSet)
            .IsRequired();

        builder.Property(x => x.TargetReps)
            .IsRequired();

        builder.Property(x => x.RestInSeconds)
            .IsRequired();

        builder.Property(x => x.SortIndex)
            .IsRequired();

        // A session cannot contain the same exercise more than once.
        builder.HasIndex(x => new
            {
                x.SessionId,
                x.ExerciseId
            })
            .IsUnique();

        builder.HasOne(x => x.Session)
            .WithMany(x => x.Exercises)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Exercise)
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Sets)
            .WithOne(x => x.WorkoutSessionExercise)
            .HasForeignKey(x => x.WorkoutSessionExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}