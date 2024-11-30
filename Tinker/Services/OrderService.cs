using Tinker.Components.DTOs;

public class OrderService
{
    private readonly CustomerService _customerService;

    public OrderService(CustomerService customerService)
    {
        _customerService = customerService;
    }

    public void ProcessOrder(OrderDTO orderDto)
    {
        // Map DTO to entity
        var order = new Order
        {
            CustomerId = orderDto.CustomerId,
            Date = orderDto.Date,
            TotalAmount = orderDto.TotalAmount,
            Items = orderDto.Items.Select(itemDto => new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                DiscountPercentage = itemDto.DiscountPercentage
            }).ToList()
        };

        // Existing order processing logic...

        // Update loyalty points
        var customer = _customerService.GetCustomerById(order.CustomerId);
        if (customer != null)
        {
            int pointsEarned = CalculateLoyaltyPoints(order.TotalAmount);
            customer.LoyaltyPoints += pointsEarned;
            customer.LastPurchaseDate = DateTime.UtcNow; // Use UTC for consistency

            // Update membership tier
            UpdateMembershipTier(customer);

            // Save changes
            _customerService.UpdateCustomer(customer);
        }
    }

    private int CalculateLoyaltyPoints(decimal amount)
    {
        // Example: 1 point per $10 spent
        return (int)(amount / 10);
    }

    private void UpdateMembershipTier(Customer customer)
    {
        customer.MembershipTier = customer.LoyaltyPoints switch
        {
            >= 1000 => "Platinum",
            >= 500 => "Gold",
            >= 200 => "Silver",
            _ => "Standard"
        };
    }
}
