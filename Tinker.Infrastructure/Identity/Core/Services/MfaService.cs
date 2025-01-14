using Tinker.Infrastructure.Abstractions.Auth;

namespace Tinker.Infrastructure.Identity.Core.Services;

public class MfaService : IMfaService
{
    private const int SecretKeyLength = 20;
    private const int WindowSize = 2;

    public Task<string> GenerateSecretKey()
    {
        var secretKey = KeyGeneration.GenerateRandomKey(SecretKeyLength);
        return Task.FromResult(Base32Encoding.ToString(secretKey));
    }

    public Task<bool> ValidateCode(string secretKey, string code)
    {
        try
        {
            var keyBytes = Base32Encoding.ToBytes(secretKey);
            var totp = new Totp(keyBytes);
            return Task.FromResult(totp.VerifyTotp(code, out _, new VerificationWindow(WindowSize)));
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<string> GenerateQrCodeUri(string secretKey, string email, string issuer)
    {
        var provisionUrl = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}?" +
                          $"secret={secretKey}&issuer={Uri.EscapeDataString(issuer)}";
        return Task.FromResult(provisionUrl);
    }
}