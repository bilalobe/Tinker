using Tinker.Core.Domain.Users.Entities;
using Tinker.Shared.DTOs.Users;

namespace Tinker.Core.Domain.Users.Extensions;

public static class UserExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id.Value,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsMfaEnabled = user.IsMfaEnabled,
            MembershipTier = user.MembershipTier,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}