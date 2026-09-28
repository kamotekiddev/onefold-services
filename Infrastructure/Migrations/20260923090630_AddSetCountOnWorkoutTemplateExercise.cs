using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSetCountOnWorkoutTemplateExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetReps",
                table: "WorkoutExercises",
                newName: "TargetRepsPerSet");

            migrationBuilder.AddColumn<int>(
                name: "SetCount",
                table: "WorkoutExercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetCount",
                table: "WorkoutExercises");

            migrationBuilder.RenameColumn(
                name: "TargetRepsPerSet",
                table: "WorkoutExercises",
                newName: "TargetReps");
        }
    }
}
