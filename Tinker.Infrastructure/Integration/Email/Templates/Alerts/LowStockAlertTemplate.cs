using Tinker.Infrastructure.Integration.Email.Templates.Base;

namespace Tinker.Infrastructure.Integration.Email.Templates.Alerts;

public class LowStockAlertTemplate : EmailTemplateBase
{
    public override string Subject => "Low Stock Alert";
    public override string TemplateName => "LowStockAlert";
    
    protected override string HtmlContent => @"
        <!DOCTYPE html>
        <html>
        <body>
            <h1>Low Stock Alert</h1>
            <p>Product: {ProductName}</p>
            <p>Reference: {ProductReference}</p>
            <p>Current Quantity: {CurrentQuantity}</p>
            <p>Minimum Threshold: {MinimumThreshold}</p>
            <hr>
            <p>Please restock this item soon.</p>
        </body>
        </html>";
}