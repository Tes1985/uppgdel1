using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uppgdel1.Data;

namespace uppgdel1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController(DataContext context) : ControllerBase
{
    private readonly DataContext _context = context;

    [HttpGet()]
    public async Task<ActionResult> ListAllSuppliers()
    {
        var suppliers = await _context.Suppliers
          .Include(sp => sp.SupplierProducts)
          .ThenInclude(p => p.Product)
          .Select(supplier => new
          {
              supplier.SupplierId,
              supplier.SupplierName,
              supplier.SupplierAddress,
              supplier.SupplierCity,
              supplier.SupplierZipcode,
              supplier.ContactPerson,
              supplier.SupplierPhone,
              supplier.Email,
              Products = supplier.SupplierProducts.Select(sp => new
              {
                  sp.Product.ProductName,
                  sp.Price
              })
          })
          .ToListAsync();
        return Ok(new { success = true, data = suppliers });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> FindSpecificSupplier(int id)
    {
        var suppliers = await _context.Suppliers
          .Where(s => s.SupplierId == id)
          .Include(sp => sp.SupplierProducts)
          .ThenInclude(p => p.Product)
          .Select(supplier => new
          {
              supplier.SupplierId,
              supplier.SupplierName,
              supplier.SupplierAddress,
              supplier.SupplierCity,
              supplier.SupplierZipcode,
              supplier.ContactPerson,
              supplier.SupplierPhone,
              supplier.Email,
              Products = supplier.SupplierProducts.Select(sp => new
              {
                  sp.Product.ItemNumber,
                  sp.Product.ProductName,
                  sp.Price,
                  sp.Product.Description
              })
          })
          .ToListAsync();
        return Ok(new { success = true, data = suppliers });
    }
}