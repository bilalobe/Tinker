using Tinker.Core.Domain.Common.Models;
using Tinker.Core.Domain.Inventory.Aggregates;
using Tinker.Core.Domain.Suppliers.ValueObjects;

namespace Tinker.Core.Domain.Suppliers.Entities;

public class Supplier(SupplierId id, string name, string contactDetails) : AggregateRoot
{
    public SupplierId Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string ContactDetails { get; private set; } = contactDetails;
    public ICollection<Product> SuppliedProducts { get; private set; } = new List<Product>();

    public void UpdateContactDetails(string contactDetails)
    {
        ContactDetails = contactDetails;
    }
}