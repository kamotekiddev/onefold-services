using Domain.Entities.Workout;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutTemplateExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutTemplateExercise> builder)
    {
        builder.HasKey(we => we.Id);

        builder.HasIndex(we => we.ExerciseId);

        builder.HasOne(we => we.Exercise)
            .WithMany(e => e.WorkoutExercises)
            .HasForeignKey(we => we.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}