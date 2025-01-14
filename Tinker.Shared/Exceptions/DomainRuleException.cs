namespace Tinker.Shared.Exceptions;

public class DomainRuleException(
    string  ruleName,
    string  message,
    string? details = null)
    : BusinessException(message,
        "DOMAIN_RULE_VIOLATION",
        new Dictionary<string, object>
        {
            ["ruleName"] = ruleName,
            ["details"] = details ?? string.Empty
        })
{
    public string RuleName { get; } = ruleName;
    public string? Details { get; } = details;

    public static DomainRuleException FromRule(
        string                              ruleName,
        string                              message,
        params (string key, object value)[] context)
    {
        var exception = new DomainRuleException(ruleName, message);
        foreach (var (key, value) in context) exception.Metadata.Add(key, value);
        return exception;
    }
}