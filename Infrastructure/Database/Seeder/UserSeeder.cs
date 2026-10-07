using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeder;

public class UserSeeder
{
    private const string Email = "dev@onefold.app";

    public static async Task SeedAsync(AppDbContext dbContext)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == Email);

        if (existingUser is not null)
            return;

        var user = User.Create(Email);

        var passwordHasher = new PasswordHasher<User>();
        var hashedPassword = passwordHasher.HashPassword(user, "password");

        user.AddCredential(Credential.Create(user.Id, CredentialProvider.Email, hashedPassword));

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();
    }
}