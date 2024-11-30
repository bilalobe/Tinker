using System.ComponentModel.DataAnnotations;

public class Supplier
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = default!;

    public string ContactDetails { get; set; } = default!;

    public ICollection<Product> SuppliedProducts { get; set; } = new List<Product>();
}
