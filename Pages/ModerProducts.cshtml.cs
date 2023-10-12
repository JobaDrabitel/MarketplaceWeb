using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Collections;
using System.Linq;
using System.Text.Json;
using Marketplace_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_Web.Pages
{

	public class ModerProductsModel : PageModel
	{
		Marketplace1Context _context = new Marketplace1Context();
		public List<Product> Products { get; private set; } = new List<Product>();

		public async Task<IActionResult> OnGetAsync()
		{
			if (HttpContext.Session.GetInt32("RoleId") < 2 || HttpContext.Session.GetInt32("RoleId") == null)
				return RedirectToPage("/Index");
			try
			{

				Products = await _context.Products.ToListAsync();
				Products.RemoveAll(product => product.UpdatedAt == null);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
			}
			return Page();
		}
	}
}

