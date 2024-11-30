public class ProductDTO
{
    public int Id { get; set; }
    public string Reference { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
}
