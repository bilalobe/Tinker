using Tinker.Infrastructure.Integration.Email.Templates.Base;

namespace Tinker.Infrastructure.Integration.Email.Templates.Orders;

public class OrderCompletionTemplate : EmailTemplateBase
{
    public override string Subject => "Order Completion";
    public override string TemplateName => "OrderCompletion";
    
    protected override string HtmlContent => @"
        <!DOCTYPE html>
        <html>
        <body>
            <h1>Order Completion</h1>
            <p>Dear {CustomerName},</p>
            <p>Your order #{OrderNumber} has been completed.</p>
            <p>Order Total: {OrderTotal}</p>
            <p>Delivery Date: {DeliveryDate}</p>
            <hr>
            <p>Thank you for choosing Tinker Pharmacy!</p>
        </body>
        </html>";
}
