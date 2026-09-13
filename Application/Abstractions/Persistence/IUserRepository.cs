using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IUserRepository
{
    void Add(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByEmailWithCredentialsAsync(string email);
}