using Application.Abstractions.Persistence;
using Domain.Entities;
using Infrastructure.Database;

namespace Infrastructure.Repository;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        db.Add(user);
        await db.SaveChangesAsync();
    }
}