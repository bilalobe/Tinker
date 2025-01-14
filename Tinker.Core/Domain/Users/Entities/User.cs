using Tinker.Core.Domain.Common.Models;
using Tinker.Core.Domain.Users.Event;
using Tinker.Core.Domain.Users.ValueObjects;

namespace Tinker.Core.Domain.Users.Entities;

public class User : AggregateRoot
{
    internal string MembershipTier;

    public User(UserId id, string userName, string email)
    {
        Id = id;
        UserName = userName;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public UserId Id { get; }
    public string UserName { get; private set; }
    public string Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool IsMfaEnabled { get; private set; }
    public string? MfaSecret { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<string> Roles { get; private set; } = new List<string>();

    public void EnableMfa(string secret)
    {
        IsMfaEnabled = true;
        MfaSecret = secret;
        AddDomainEvent(new MfaEnabledEvent(Id));
    }

    private void AddDomainEvent(MfaEnabledEvent mfaEnabledEvent)
    {
        throw new NotImplementedException();
    }

    public void DisableMfa()
    {
        IsMfaEnabled = false;
        MfaSecret = null;
        AddDomainEvent(new MfaDisabledEvent(Id));
    }

    private void AddDomainEvent(MfaDisabledEvent mfaDisabledEvent)
    {
        throw new NotImplementedException();
    }

    public void UpdateProfile(string? phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}