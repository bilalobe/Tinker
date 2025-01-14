namespace Tinker.Infrastructure.Configuration.Base;

public static class ValidationMessages
{
    public const string InvalidConnection = "Invalid connection string";
    public const string InvalidTimespan = "Invalid timespan value";
    public const string RequiredField = "Field is required";
    public const string InvalidRange = "Value is outside allowed range";
    public const string InvalidFormat = "Invalid format";
    public const string InvalidUrl = "Invalid URL format";
    public const string InvalidEmail = "Invalid email format";
    public const string InvalidPort = "Port must be between 1 and 65535";
    public const string InvalidPath = "Invalid file path";
    public const string InvalidThreshold = "Invalid threshold value";
    public const string DependentSetting = "Setting requires {0} to be enabled";
}