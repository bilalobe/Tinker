namespace Tinker.Infrastructure.Integration.Email.Templates.Base;

public abstract class EmailTemplateBase
{
    public abstract string Subject { get; }
    public abstract string TemplateName { get; }
    protected abstract string HtmlContent { get; }

    public virtual string BuildBody(Dictionary<string, string> parameters)
    {
        var content = HtmlContent;
        foreach (var param in parameters)
        {
            content = content.Replace($"{{{param.Key}}}", param.Value);
        }
        return content;
    }
}