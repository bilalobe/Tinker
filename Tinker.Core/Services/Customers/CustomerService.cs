using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Customers.Entities;
using Tinker.Core.Domain.Customers.Repositories;
using Tinker.Core.Domain.Customers.ValueObjects;
using Tinker.Core.Services.Customers.Interfaces;
using Tinker.Shared.DTOs.Customers;
using Tinker.Shared.Exceptions;

namespace Tinker.Core.Services.Customers;

public partial class CustomerService(
    ICustomerRepository customerRepository,
    ILogger<CustomerService> logger)
    : ICustomerService
{
    public async Task<IEnumerable<CustomerDto>> GetCustomers()
    {
        var customers = await customerRepository.GetAllAsync();
        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            LoyaltyPoints = c.LoyaltyPoints,
            MembershipTier = c.MembershipTier
        });
    }

    public async Task AddCustomer(CustomerDto customerDto)
    {
        ValidateCustomer(customerDto);

        if (await customerRepository.ExistsByEmailAsync(customerDto.Email))
            throw new ValidationException($"Customer with email {customerDto.Email} already exists");

        var customer = new Customer
        {
            Id = customerDto.Id,
            Name = customerDto.Name,
            Email = customerDto.Email,
            PhoneNumber = customerDto.PhoneNumber,
            LoyaltyPoints = 0,
            MembershipTier = "Standard"
        };

        await customerRepository.AddAsync(customer);
        logger.LogInformation("Customer {Email} added successfully", customer.Email);
    }

    public async Task UpdateCustomer(CustomerDto customerDto)
    {
        ValidateCustomer(customerDto);

        var existingCustomer = await customerRepository.GetByIdAsync(customerDto.Id)
                               ?? throw new NotFoundException($"Customer with ID {customerDto.Id} not found");

        if (await customerRepository.ExistsByEmailAsync(customerDto.Email, customerDto.Id))
            throw new ValidationException($"Email {customerDto.Email} is already in use");

        existingCustomer.Name = customerDto.Name;
        existingCustomer.Email = customerDto.Email;
        existingCustomer.PhoneNumber = customerDto.PhoneNumber;

        await customerRepository.UpdateAsync(existingCustomer);
        logger.LogInformation("Customer {Email} updated successfully", existingCustomer.Email);
    }

    public async Task<CustomerDto?> GetCustomerById(int id)
    {
        var customer = await customerRepository.GetByIdAsync(id);
        if (customer == null) return null;

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            LoyaltyPoints = customer.LoyaltyPoints,
            MembershipTier = customer.MembershipTier
        };
    }

    public async Task DeleteCustomer(int id)
    {
        var customer = await customerRepository.GetByIdAsync(id)
                       ?? throw new NotFoundException($"Customer {id} not found");

        if (await customerRepository.HasActiveOrdersAsync(customer.Id))
            throw new ValidationException("Cannot delete customer with active orders");

        await customerRepository.DeleteAsync(customer);
    }

    public async Task<IEnumerable<CustomerDto>> SearchCustomers(string searchTerm)
    {
        var customers = await customerRepository.SearchAsync(searchTerm);
        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            LoyaltyPoints = c.LoyaltyPoints,
            MembershipTier = c.MembershipTier
        });
    }

    public async Task<CustomerStatistics> GetCustomerStatistics(int customerId)
    {
        var customer = await customerRepository.GetByIdWithOrdersAsync(customerId)
                       ?? throw new NotFoundException($"Customer {customerId} not found");

        var orders = customer.Orders.ToList();

        return new CustomerStatistics
        {
            TotalOrders = orders.Count,
            TotalSpent = orders.Sum(o => o.TotalAmount),
            LoyaltyPoints = customer.LoyaltyPoints,
            MembershipTier = customer.MembershipTier,
            LastPurchaseDate = orders.Max(o => o.Date)
        };
    }

    public async Task<Customer?> GetCustomerEntityById(int id)
    {
        return await customerRepository.GetByIdAsync(id);
    }

    public async Task UpdateCustomerEntity(Customer customer)
    {
        await customerRepository.UpdateAsync(customer);
    }

    private void ValidateCustomer(CustomerDto customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new ValidationException("Customer name is required");

        if (string.IsNullOrWhiteSpace(customer.Email) || !IsValidEmail(customer.Email))
            throw new ValidationException("Valid email address is required");

        if (string.IsNullOrWhiteSpace(customer.PhoneNumber) || !IsValidPhoneNumber(customer.PhoneNumber))
            throw new ValidationException("Valid phone number is required");
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        return MyRegex().IsMatch(phoneNumber);
    }

    public async Task CreateCustomer(Customer customer)
    {
        await customerRepository.AddAsync(customer);
        logger.LogInformation("Customer {CustomerId} created successfully", customer.Id);
    }

    public async Task UpdateLoyaltyPoints(CustomerId customerId, int points)
    {
        var customer = await customerRepository.GetByIdAsync(customerId.Value);
        if (customer == null)
            throw new NotFoundException($"Customer {customerId} not found");

        customer.UpdateLoyaltyPoints(points);
        await customerRepository.UpdateAsync(customer);
        logger.LogInformation("Loyalty points updated for customer {CustomerId}", customerId);
    }

    [GeneratedRegex(@"^\+?[\d\s-()]+$")]
    private static partial Regex MyRegex();
}