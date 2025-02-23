using dagnysbageri.Entities;
using dagnysbageri.ViewModels;
using dagnysbageri.ViewModels.Address;
using dagnysbageri.ViewModels.Customer;
using Microsoft.EntityFrameworkCore;
using dagnysbageri.Data;
using dagnysbageri.Interfaces;
namespace dagnysbageri.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DataContext _context;
    private readonly IAddressRepository _repo;
    public CustomerRepository(DataContext context, IAddressRepository repo)
    {
        _repo = repo;
        _context = context;

    }
    public async Task<bool> Add(CustomerPostViewModel model)
    {
        try
        {
            var customerexists = await _context.Customers
                .FirstOrDefaultAsync(c => c.CompanyName.ToLower().Trim() == model.CompanyName.ToLower().Trim());

            if (customerexists is not null)
            {
                throw new Exception("Kunden finns redan!!");
            }

            var customer = new Customer
            {
                CompanyName = model.CompanyName,
                Phone = model.Phone,
                Email = model.Email,
                ContactPerson = model.ContactPerson,
                CustomerAddresses = new List<CustomerAddress>()
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            foreach (var addressModel in model.Addresses)
            {
                var address = await _repo.AddOne(addressModel);

                customer.CustomerAddresses.Add(new CustomerAddress
                {
                    CustomerId = customer.Id,
                    AddressId = address.Id
                });
            }

            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fel inträff: {ex.Message}");
        }
    }

    public async Task<CustomerViewModel> GetOne(int id)
    {
        try
        {
            var customer = await _context.Customers
              .Where(c => c.Id == id)
              .Include(c => c.CustomerAddresses)
                .ThenInclude(c => c.Address)
                .ThenInclude(c => c.PostalAddress)
              .Include(c => c.CustomerAddresses)
                .ThenInclude(c => c.Address)
                .ThenInclude(c => c.AddressType)
                .Include(c => c.Orders)
                .ThenInclude(so => so.OrderItems)
                .ThenInclude(so => so.SalesProduct)
              .SingleOrDefaultAsync();

            if (customer is null)
            {
                throw new Exception($"Det finns ingen kund med id {id}");
            }

            var view = new CustomerViewModel
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                Phone = customer.Phone,
                Email = customer.Email,
                ContactPerson = customer.ContactPerson
            };

            var addresses = customer.CustomerAddresses.Select(c => new AddressViewModel
            {
                AddressLine = c.Address.AddressLine,
                PostalCode = c.Address.PostalAddress.PostalCode,
                City = c.Address.PostalAddress.City,
                AddressType = c.Address.AddressType.Value
            });

            view.Addresses = [.. addresses];

            var orders = customer.Orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                OrderItems = o.OrderItems.Select(oi => new OrderItemViewModel
                {
                    SalesProductName = oi.SalesProduct.ProductName,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            }).ToList();

            view.Orders = orders;
            return view;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IList<CustomersViewModel>> GetAll()
    {
        var response = await _context.Customers.ToListAsync();
        var customers = response.Select(c => new CustomersViewModel
        {
            Id = c.Id,
            CompanyName = c.CompanyName,
            Phone = c.Phone,
            Email = c.Email,
            ContactPerson = c.ContactPerson
        });

        return [.. customers];
    }

    public async Task<bool> UpdatePerson(int id, UpdateContactPersonViewModel model)
    {
        try
        {
            var customer = await _context.Customers.SingleOrDefaultAsync(c => c.Id == id);

            if (customer is null)
            {
                throw new Exception($"Det finns ingen kund med id {id}");
            }

            customer.ContactPerson = model.ContactPerson;

            _context.Customers.Update(customer);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }

    }
}