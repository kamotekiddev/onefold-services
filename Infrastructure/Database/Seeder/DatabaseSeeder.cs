namespace Infrastructure.Database.Seeder;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        await UserSeeder.SeedAsync(dbContext);
        await ExerciseSeeder.SeedAsync(dbContext);
        await WorkoutTemplateSeeder.SeedAsync(dbContext);
    }
}