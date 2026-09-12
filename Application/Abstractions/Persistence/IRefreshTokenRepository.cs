using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);
}