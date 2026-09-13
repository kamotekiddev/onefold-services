using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);
    Task<RefreshToken?> GetByValue(string refreshToken);
    Task<RefreshToken?> GetByValueWithUser(string refreshToken);
}