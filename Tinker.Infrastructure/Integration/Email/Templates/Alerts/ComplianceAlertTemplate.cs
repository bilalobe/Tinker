using Tinker.Infrastructure.Integration.Email.Templates.Base;

namespace Tinker.Infrastructure.Integration.Email.Templates.Alerts;

public class ComplianceAlertTemplate : EmailTemplateBase
{
    public override string Subject => "Compliance Alert";
    public override string TemplateName => "ComplianceAlert";
    
    protected override string HtmlContent => @"
        <!DOCTYPE html>
        <html>
        <body>
            <h1>Compliance Alert</h1>
            <p>Alert Type: {AlertType}</p>
            <p>Reference: {Reference}</p>
            <p>Details: {Details}</p>
            <hr>
            <p style='color: red;'>This alert requires immediate attention.</p>
        </body>
        </html>";
}