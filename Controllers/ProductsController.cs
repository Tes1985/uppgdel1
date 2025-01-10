using uppgdel1.Data;
using uppgdel1.Entities;
using uppgdel1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace uppgdel1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(DataContext context) : ControllerBase
{
  private readonly DataContext _context = context;

  [HttpGet("{id}")]
  public async Task<ActionResult> FindSpecificProduct(int id)
  {
    var product = await _context.Products
      .Where(c => c.ProductId == id)
      .Include(p => p.SupplierProducts)
      .ThenInclude(sp => sp.Supplier)
      .Select(product => new
      {
        product.ProductId,
        product.ItemNumber,
        product.ProductName,
        product.Description,
        Supplierproducts = product.SupplierProducts.Select(sp => new
        {
          sp.Supplier.SupplierName,
          sp.Price
        })
      }
      )
      .ToListAsync();
    return Ok(new { success = true, product });
  }

  [HttpPost("{id}")]
  public async Task<ActionResult> CreateProduct(int id, ProductViewModel product)
  {
    var supplier = await _context.Suppliers
        .Where(s => s.SupplierId == id)
        .SingleOrDefaultAsync();

    if (supplier == null)
    {
      return BadRequest(new { success = false, StatusCode = 400, message = "Supplier was not found!" });
    }
    var existingProduct = await _context.Products
        .Where(p => p.ItemNumber == product.ItemNumber)
        .SingleOrDefaultAsync();
    if (existingProduct != null)
    {
      return BadRequest(new { success = false, StatusCode = 400, message = "This Product already exists!" });
    }

    var newProduct = new Product
    {
      ItemNumber = product.ItemNumber,
      ProductName = product.ProductName,
      Image = product.Image,
      Description = product.Description
    };

    _context.Products.Add(newProduct);

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateException)
    {
      return BadRequest(new { success = false, StatusCode = 400, message = "Something went wrong!" });
    }

    var newSupplierproduct = new SupplierProduct
    {
      SupplierId = id,
      ProductId = newProduct.ProductId,
      Price = product.Price
    };

    _context.SupplierProducts.Add(newSupplierproduct);

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateException)
    {
      return BadRequest(new { success = false, StatusCode = 400, message = "Something went wrong!" });
    }
    return CreatedAtAction(nameof(FindSpecificProduct), new { id = newProduct.ProductId }, new { success = true, StatusCode = 201, data = newProduct });
  }

  [HttpPatch("{supplierid}/{productid}")]
  public async Task<ActionResult> UpdatePrice(int supplierid, int productid, ChangePriceViewModel product)
  {
    var priceToUpdate = await _context.SupplierProducts
        .Where(x => x.SupplierId == supplierid && x.ProductId == productid)
        .SingleOrDefaultAsync();

    if (priceToUpdate == null)
    {
      return BadRequest(new { success = false, StatusCode = 400, message = "Product could not be found!" });
    }
    priceToUpdate.Price = product.Price;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateException)
    {
      return BadRequest(new { success = false, StatusCode = 400, message = "Something went wrong!" });
    }

    return Ok(new {success = true, message = "Price has been updated!"});
  }

}