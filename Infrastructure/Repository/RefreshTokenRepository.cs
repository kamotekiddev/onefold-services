using Application.Abstractions.Persistence;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    public void Add(RefreshToken refreshToken)
    {
        db.RefreshTokens.Add(refreshToken);
    }

    public async Task<RefreshToken?> GetByValue(string refreshToken)
    {
        return await db.RefreshTokens.SingleOrDefaultAsync(r => r.Value == refreshToken);
    }

    public async Task<RefreshToken?> GetByValueWithUser(string refreshToken)
    {
        return await db.RefreshTokens.Include(r => r.User).SingleOrDefaultAsync(r => r.Value == refreshToken);
    }
}