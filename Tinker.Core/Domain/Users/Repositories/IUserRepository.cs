using Tinker.Core.Domain.Users.Entities;
using Tinker.Core.Domain.Users.ValueObjects;

namespace Tinker.Core.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId       id);
    Task<User?> GetByEmailAsync(string    email);
    Task<User?> GetByUserNameAsync(string userName);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User                      user);
    Task UpdateAsync(User                   user);
    Task DeleteAsync(UserId                 id);
    Task<bool> ExistsByEmailAsync(string    email);
    Task<bool> ExistsByUserNameAsync(string userName);
}