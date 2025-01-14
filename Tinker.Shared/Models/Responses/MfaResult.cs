namespace Tinker.Shared.Models.Responses;
public class MfaResult
{
    private MfaResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static MfaResult Failed(string errorMessage)
    {
        return new MfaResult(false, errorMessage);
    }

    public static MfaResult Success()
    {
        return new MfaResult(true);
    }
}