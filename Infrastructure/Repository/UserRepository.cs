using Application.Abstractions.Persistence;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public void Add(User user)
    {
        db.Users.Add(user);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await db.Users.SingleOrDefaultAsync(u => u.Email == email);
    }
}