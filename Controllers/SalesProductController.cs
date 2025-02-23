using dagnysbageri.Data;
using dagnysbageri.Entities;
using dagnysbageri.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dagnysbageri.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesProductsController : ControllerBase
{
    private readonly DataContext _context;
    public SalesProductsController(DataContext context)
    {
        _context = context;

    }

    [HttpGet()]

    public async Task<ActionResult> ListAll()
    {
        var salesProducts = await _context.SalesProducts.ToListAsync();
        return Ok(new { success = true, data = salesProducts });
    }

    [HttpGet("{id}")]

    public async Task<ActionResult> GetOne(int id)
    {
        var salesProduct = await _context.SalesProducts
        .Where(sp => sp.Id == id)
        .SingleOrDefaultAsync();

        if (salesProduct is null)
        {
            return NotFound(new { success = false, message = $"Tyvärr kunde vi inte hitta någon produkt med id {id}" });
        }

        return Ok(new { success = true, data = salesProduct });
    }

    [HttpPost()]

    public async Task<ActionResult> AddSalesProduct(SalesProductPostViewModel model)
    {
        var prod = await _context.SalesProducts.FirstOrDefaultAsync(p => p.ItemNumber == model.ItemNumber);

        if (prod != null)
        {
            return BadRequest(new { success = false, message = $"Produkten existerar redan {0}", model.ProductName });
        }


        var salesProduct = new SalesProduct
        {
            ItemNumber = model.ItemNumber,
            ProductName = model.ProductName,
            EachPrice = model.EachPrice,
            Weight = model.Weight,
            PackAmount = model.PackAmount,
            ManufactureDate = model.ManufactureDate,
            BestBeforeDate = model.BestBeforeDate
        };

        try
        {
            await _context.SalesProducts.AddAsync(salesProduct);
            await _context.SaveChangesAsync();

            // return Ok(new {success=true, data = product});
            return CreatedAtAction(nameof(GetOne), new { id = salesProduct.Id }, salesProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }

    [HttpPatch("{id}")]

    public async Task<ActionResult> UpdatePrice(int id, [FromQuery] double eachPrice)
    {
        var prod = await _context.SalesProducts.FirstOrDefaultAsync(p => p.Id == id);

        if (prod == null)
        {
            return NotFound(new { success = false, message = $"Produkten som du försöker uppdatera existerar inte längre {0}", id });
        }

        prod.EachPrice = eachPrice;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

        return NoContent();
    }
}
