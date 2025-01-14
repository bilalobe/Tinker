using Tinker.Infrastructure.Integration.Email.Templates.Base;

namespace Tinker.Infrastructure.Integration.Email.Templates.Alerts;

public class ExpiryAlertTemplate : EmailTemplateBase
{
    public override string Subject => "Expiry Alert";
    public override string TemplateName => "ExpiryAlert";
    
    protected override string HtmlContent => @"
        <!DOCTYPE html>
        <html>
        <body>
            <h1>Expiry Alert</h1>
            <p>Product: {ProductName}</p>
            <p>Reference: {ProductReference}</p>
            <p>Expiry Date: {ExpiryDate}</p>
            <hr>
            <p>Please take necessary actions before the expiry date.</p>
        </body>
        </html>";
}
