namespace Tinker.Core.Security.Interfaces;

public interface IAuthenticationService
{
    Task<AuthResult> ValidateCredentials(string username, string password);
    Task<AuthResult> ValidateMfaCode(string     userId,   string code);
}