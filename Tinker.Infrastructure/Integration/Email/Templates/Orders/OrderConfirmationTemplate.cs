using Tinker.Infrastructure.Integration.Email.Templates.Base;

namespace Tinker.Infrastructure.Integration.Email.Templates.Orders;

public class OrderConfirmationTemplate : EmailTemplateBase
{
    public override string Subject => "Order Confirmation";
    public override string TemplateName => "OrderConfirmation";
    
    protected override string HtmlContent => @"
        <!DOCTYPE html>
        <html>
        <body>
            <h1>Order Confirmation</h1>
            <p>Dear {CustomerName},</p>
            <p>Your order #{OrderNumber} has been confirmed.</p>
            <p>Order Total: {OrderTotal}</p>
            <p>Expected Delivery: {DeliveryDate}</p>
            <hr>
            <p>Thank you for choosing Tinker Pharmacy!</p>
        </body>
        </html>";
}