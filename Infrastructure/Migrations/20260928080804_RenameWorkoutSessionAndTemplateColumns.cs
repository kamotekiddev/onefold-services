using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameWorkoutSessionAndTemplateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessionExercises_Exercises_ExerciseId",
                table: "WorkoutSessionExercises");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSets_WorkoutSessionExerciseId",
                table: "WorkoutSets");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_UserId",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessionExercises_SessionId",
                table: "WorkoutSessionExercises");

            migrationBuilder.RenameColumn(
                name: "TargetRepsPerSet",
                table: "WorkoutExercises",
                newName: "TargetSet");

            migrationBuilder.RenameColumn(
                name: "SetCount",
                table: "WorkoutExercises",
                newName: "TargetReps");

            migrationBuilder.RenameColumn(
                name: "RestPerSetInSeconds",
                table: "WorkoutExercises",
                newName: "SortIndex");

            migrationBuilder.RenameColumn(
                name: "OrderIndex",
                table: "WorkoutExercises",
                newName: "RestInSeconds");

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "WorkoutSets",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "WorkoutSessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_WorkoutSessionExerciseId_SetNumber",
                table: "WorkoutSets",
                columns: new[] { "WorkoutSessionExerciseId", "SetNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_UserId_WorkoutTemplateId",
                table: "WorkoutSessions",
                columns: new[] { "UserId", "WorkoutTemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_WorkoutTemplateId",
                table: "WorkoutSessions",
                column: "WorkoutTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessionExercises_SessionId_ExerciseId",
                table: "WorkoutSessionExercises",
                columns: new[] { "SessionId", "ExerciseId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessionExercises_Exercises_ExerciseId",
                table: "WorkoutSessionExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessions_WorkoutTemplates_WorkoutTemplateId",
                table: "WorkoutSessions",
                column: "WorkoutTemplateId",
                principalTable: "WorkoutTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessionExercises_Exercises_ExerciseId",
                table: "WorkoutSessionExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessions_WorkoutTemplates_WorkoutTemplateId",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSets_WorkoutSessionExerciseId_SetNumber",
                table: "WorkoutSets");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_UserId_WorkoutTemplateId",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_WorkoutTemplateId",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessionExercises_SessionId_ExerciseId",
                table: "WorkoutSessionExercises");

            migrationBuilder.RenameColumn(
                name: "TargetSet",
                table: "WorkoutExercises",
                newName: "TargetRepsPerSet");

            migrationBuilder.RenameColumn(
                name: "TargetReps",
                table: "WorkoutExercises",
                newName: "SetCount");

            migrationBuilder.RenameColumn(
                name: "SortIndex",
                table: "WorkoutExercises",
                newName: "RestPerSetInSeconds");

            migrationBuilder.RenameColumn(
                name: "RestInSeconds",
                table: "WorkoutExercises",
                newName: "OrderIndex");

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "WorkoutSets",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "WorkoutSessions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_WorkoutSessionExerciseId",
                table: "WorkoutSets",
                column: "WorkoutSessionExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_UserId",
                table: "WorkoutSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessionExercises_SessionId",
                table: "WorkoutSessionExercises",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessionExercises_Exercises_ExerciseId",
                table: "WorkoutSessionExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
