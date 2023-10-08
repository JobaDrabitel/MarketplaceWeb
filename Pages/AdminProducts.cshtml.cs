using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Collections;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Marketplace_Web.Models;

namespace Marketplace_Web.Pages
{

	public class AdminProductsModel : PageModel
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
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
			}
			return Page();
		}
		[BindProperty]
		public int ItemId { get; set; }

		public string DeleteResult { get; set; }

		public async Task<IActionResult> OnPostAsync()
		{
			ProductController productController = new ProductController(_context);
			try
			{
				var product = productController.DeleteProduct(ItemId);
			}
			catch (Exception ex)
			{
				// Обработайте другие исключения, если необходимо
				ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
			}
			return await OnGetAsync();
		}
	}
}

