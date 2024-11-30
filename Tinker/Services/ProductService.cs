public class ProductService
{
    private readonly List<Product> _products = new();

    public IEnumerable<Product> GetProducts() => _products;

    public void AddProduct(Product product) => _products.Add(product);

    public void UpdateProduct(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index >= 0) _products[index] = product;
    }

    public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);
}
