using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
	private readonly Marketplace1Context _context;

	public CategoryController(Marketplace1Context context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<IEnumerable<Category>> GetCategories()
	{
		return await _context.Categories.ToListAsync();
	}

	[HttpGet("{id}")]
	public async Task<Category> GetCategory(int id)
	{
		return await _context.Categories.FindAsync(id);
	}

	[HttpPost]
	public async Task<Category> PostCategory(Category Category)
	{
		_context.Categories.Add(Category);
		await _context.SaveChangesAsync();

		return Category;
	}

	[HttpPut("{id}")]
	public async Task<Category> PutCategory(int id, Category Category)
	{
		if (id != Category.CategoryId)
		{
			return null;
		}

		_context.Entry(Category).State = EntityState.Modified;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!CategoryExists(id))
			{
				return null;
			}
			else
			{
				throw;
			}
		}

		return Category;
	}

	[HttpDelete("{id}")]
	public async Task<Category> DeleteCategory(int id)
	{
		var Category = await _context.Categories.FindAsync(id);
		if (Category == null)
		{
			return null;
		}

		_context.Categories.Remove(Category);
		await _context.SaveChangesAsync();

		return Category;
	}

	private bool CategoryExists(int id)
	{
		return _context.Categories.Any(e => e.CategoryId == id);
	}
}
