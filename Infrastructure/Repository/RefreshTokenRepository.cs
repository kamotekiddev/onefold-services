using Application.Abstractions.Persistence;
using Domain.Entities;
using Infrastructure.Database;

namespace Infrastructure.Repository;

public class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    public void Add(RefreshToken refreshToken)
    {
        db.RefreshTokens.Add(refreshToken);
    }
}