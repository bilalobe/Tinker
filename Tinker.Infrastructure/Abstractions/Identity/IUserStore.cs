using Tinker.Infrastructure.Identity.Core.Models;

namespace Tinker.Infrastructure.Abstractions.Identity;

public interface IUserStore
{
    Task<ApplicationUser> FindByIdAsync(string userId);
    Task<bool> EnableMfaAsync(string userId);
}