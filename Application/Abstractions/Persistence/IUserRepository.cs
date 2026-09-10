using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IUserRepository
{
    public Task AddAsync(User user);
}