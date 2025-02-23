using dagnysbageri.Data;
using dagnysbageri.Entities;
using dagnysbageri.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dagnysbageri.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly DataContext _context;
    public OrdersController(DataContext context)
    {
        _context = context;

    }

    [HttpGet()]

    public async Task<ActionResult> ListAll()
    {
        var orders = await _context.Orders
        .Include(so => so.OrderItems)
        .ThenInclude(oi => oi.SalesProduct)
        .Include(so => so.Customer)
        .Select(order => new
        {
            OrderNumber = order.Id,
            order.OrderDate,
            Customer = new
            {
                order.Customer.Id,
                order.Customer.CompanyName,
                order.Customer.ContactPerson,
                order.Customer.Email,
                order.Customer.Phone
            },
            Items = order.OrderItems
            .Select(oi => new
            {
                oi.SalesProduct.ProductName,
                oi.SalesProduct.PackAmount,
                oi.SalesProduct.EachPrice,
                PackPrice = oi.SalesProduct.PackAmount * oi.SalesProduct.EachPrice,
                oi.SalesProduct.Weight,
                oi.Quantity,
                TotalPrice = oi.Quantity * (oi.SalesProduct.PackAmount * oi.SalesProduct.EachPrice),
            })
        })
        .ToListAsync();

        return Ok(new { success = true, data = orders });
    }

    [HttpGet("{id}")]

    public async Task<ActionResult> FindOne(int id)
    {
        try
        {
            var order = await _context.Orders
            .Where(so => so.Id == id)
            .Include(o => o.OrderItems)
            .Select(so => new
            {
                OrderNumber = so.Id,
                so.OrderDate,
                Customer = new
                {
                    so.Customer.Id,
                    so.Customer.CompanyName,
                    so.Customer.ContactPerson,
                    so.Customer.Email,
                    so.Customer.Phone
                },
                Items = so.OrderItems
                .Select(oi => new
                {
                    oi.SalesProduct.ProductName,
                    oi.SalesProduct.PackAmount,
                    oi.SalesProduct.EachPrice,
                    PackPrice = oi.SalesProduct.PackAmount * oi.SalesProduct.EachPrice,
                    oi.SalesProduct.Weight,
                    oi.Quantity,
                    TotalPrice = oi.Quantity * (oi.SalesProduct.PackAmount * oi.SalesProduct.EachPrice)
                })
            })
            .SingleOrDefaultAsync();

            if (order is null)
            {
                return NotFound(new { success = false, message = $"Vi kunde inte hitta någon beställning med id {id}" });
            }
            return Ok(new { success = true, data = order });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ett fel inträffade.", error = ex.Message });
        }
    }

    [HttpGet("date/{date}")]

    public async Task<ActionResult> FindByDate(DateOnly date)
    {
        var result = await _context.Orders
        .Where(s => s.OrderDate == date)
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.SalesProduct)
        .Select(s => new
        {
            OrderNumber = s.Id,
            s.OrderDate,
            Customer = new
            {
                s.Customer.Id,
                s.Customer.CompanyName,
                s.Customer.ContactPerson,
                s.Customer.Email,
                s.Customer.Phone
            },
            Items = s.OrderItems
            .Select(oi => new
            {
                oi.SalesProduct.ProductName,
                oi.SalesProduct.PackAmount,
                oi.SalesProduct.EachPrice,
                PackPrice = oi.SalesProduct.PackAmount * oi.SalesProduct.EachPrice,
                oi.SalesProduct.Weight,
                oi.Quantity,
                TotalPrice = oi.Quantity * (oi.SalesProduct.PackAmount * oi.SalesProduct.EachPrice)
            })
        })
        .ToListAsync();

        if (result is null)
        {
            return NotFound(new { success = false, message = $"Vi kunde inte hitta någon beställning med datum {date}" });
        }
        return Ok(new { success = true, data = result });
    }

    [HttpPost]
    public async Task<ActionResult> AddOrder(OrderPostViewModel order)
    {
        // Kontrollera om kunden finns
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == order.CustomerId);
        if (customer == null)
        {
            return NotFound(new { success = false, message = "Kunden existerar inte." });
        }

        var order1 = new Order
        {
            CustomerId = order.CustomerId,
            OrderDate = DateOnly.FromDateTime(DateTime.Now),
            OrderItems = new List<OrderItem>()
        };

        foreach (var item in order.OrderItems)
        {
            var product = await _context.SalesProducts.FirstOrDefaultAsync(p => p.ProductName.ToUpper() == item.SalesProductName.ToUpper());
            if (product == null)
            {
                return NotFound(new { success = false, message = $"Produkten '{item.SalesProductName}' hittades inte." });
            }

            var orderItem = new OrderItem
            {
                SalesProductId = product.Id,
                Quantity = item.Quantity,
                Price = product.EachPrice * item.Quantity,
            };
        }

        _context.Orders.Add(order1);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(AddOrder), new { id = order1.Id }, new { success = true, orderId = order1.Id });
    }


}
