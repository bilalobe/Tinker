using System.ComponentModel;
using System.Reflection.Metadata;
using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Suppliers.Entities;
using Tinker.Core.Domain.Suppliers.Repositories;
using Tinker.Core.Services.Invoices.Interfaces;
using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Services.Invoices;

public class InvoiceService(
    ISupplierRepository     supplierRepository,
    ILogger<InvoiceService> logger)
    : IInvoiceService
{
    public async Task<byte[]> GenerateInvoicePdf(OrderDto order)
    {
        var supplier = await supplierRepository.GetFirstAsync();
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.Content().Element(c => BuildContent(c, order, supplier));
            });
        });

        LogInvoiceGeneration(order);

        return document.GeneratePdf();
    }

    private void BuildContent(IContainer container, OrderDto order, Supplier? supplier)
    {
        container.Column(column =>
        {
            column.Spacing(10);

            // Company Information
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(supplier?.Name ?? "Your Company Name").FontSize(20).SemiBold();
                    if (!string.IsNullOrEmpty(supplier?.ContactDetails))
                    {
                        var contacts = supplier.ContactDetails.Split('\n');
                        foreach (var line in contacts) col.Item().Text(line);
                    }
                    else
                    {
                        col.Item().Text("123 Main Street");
                        col.Item().Text("City, State ZIP");
                        col.Item().Text("Phone: (123) 456-7890");
                    }
                });

                row.ConstantItem(100).Height(50).Placeholder();
            });

            // Invoice Title and Number
            column.Item().Text($"Invoice #{order.Id}").FontSize(18).Bold();

            // Date
            column.Item().Text($"Date: {order.Date:MM/dd/yyyy}");

            // Customer Information
            column.Item().Text("Bill To:").Bold();
            column.Item().Text($"{order.CustomerName}");
            // Add additional customer details if available

            // Order Items Table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(); // Description
                    columns.ConstantColumn(50); // Quantity
                    columns.ConstantColumn(80); // Unit Price
                    columns.ConstantColumn(80); // Total
                });

                // Table Header
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Description");
                    header.Cell().Element(CellStyle).AlignCenter().Text("Qty");
                    header.Cell().Element(CellStyle).AlignRight().Text("Unit Price");
                    header.Cell().Element(CellStyle).AlignRight().Text("Total");
                });

                // Table Rows
                foreach (var item in order.Items)
                {
                    table.Cell().Element(CellStyle).Text(item.ProductName);
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text(item.UnitPrice.ToString("C"));
                    table.Cell().Element(CellStyle).AlignRight().Text((item.Quantity * item.UnitPrice).ToString("C"));
                }
            });

            // Total Amount
            column.Item().AlignRight().Text($"Total: {order.TotalAmount.ToString("C")}").FontSize(16).Bold();

            // Footer
            column.Item().Text("Thank you for your business!");
        });
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).PaddingVertical(5);
    }

    private void LogInvoiceGeneration(OrderDto order)
    {
        logger.LogInformation("Generated invoice for order {OrderId}", order.Id);
    }
}