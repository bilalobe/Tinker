namespace Tinker.Infrastructure.Abstractions.Auth;

public interface IMfaService
{
    Task<string> GenerateSecretKey();
    Task<bool> ValidateCode(string secretKey, string code);
    Task<string> GenerateQrCodeUri(string secretKey, string email, string issuer);
}