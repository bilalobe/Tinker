public class OrderDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDTO> Items { get; set; } = new();
}
