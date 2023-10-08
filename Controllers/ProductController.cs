using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
	private readonly Marketplace1Context _context;

	public ProductController(Marketplace1Context context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<IEnumerable<Product>> GetProducts()
	{
		return await _context.Products.ToListAsync();
	}

	[HttpGet("{id}")]
	public async Task<Product> GetProduct(int id)
	{
		return await _context.Products.FindAsync(id);
	}

	[HttpPost]
	public async Task<Product> PostProduct(Product Product)
	{
		_context.Products.Add(Product);
		await _context.SaveChangesAsync();

		return Product;
	}

	[HttpPut("{id}")]
	public async Task<Product> PutProduct(int id, Product Product)
	{
		if (id != Product.ProductId)
		{
			return null;
		}

		_context.Entry(Product).State = EntityState.Modified;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!ProductExists(id))
			{
				return null;
			}
			else
			{
				throw;
			}
		}

		return Product;
	}

	[HttpDelete("{id}")]
	public async Task<Product> DeleteProduct(int id)
	{
		var Product = await _context.Products.FindAsync(id);
		if (Product == null)
		{
			return null;
		}

		_context.Products.Remove(Product);
		await _context.SaveChangesAsync();

		return Product;
	}

	private bool ProductExists(int id)
	{
		return _context.Products.Any(e => e.ProductId == id);
	}
}
