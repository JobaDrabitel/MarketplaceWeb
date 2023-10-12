using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Marketplace_Web.Pages
{
	public class ModeratorApproveProductsModel : PageModel
	{
		public string ApproveResult { get; set; }

		public List<Product> Products { get; private set; } = new List<Product>();

		Marketplace1Context _context = new Marketplace1Context();
		public async Task<IActionResult> OnGetAsync()
		{
			if (HttpContext.Session.GetInt32("RoleId") < 2 || HttpContext.Session.GetInt32("RoleId") == null)
				return RedirectToPage("/Index");
			try
			{

				Products = await _context.Products.ToListAsync();
				Products.RemoveAll(product => product.UpdatedAt != null);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
			}
			return Page();
		}
		public async Task OnPostApproveProductAsync(string id)
		{
			ProductController productController = new ProductController(_context);
			try
			{
				var productId = Convert.ToInt32(id);
				var product = await productController.GetProduct(productId);
				product.UpdatedAt = DateTime.Now;
				product = await productController.PutProduct(productId, product);
				
			}
			catch (Exception ex)
			{
				// Обработайте другие исключения, если необходимо
				ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
			}
			await OnGetAsync();
		}
		public async Task OnPostRejectProductAsync(string id)
		{
			ProductController productController = new ProductController(_context);
			try
			{
				var productId = Convert.ToInt32(id);
				var product = await productController.DeleteProduct(productId);

			}
			catch (Exception ex)
			{
				// Обработайте другие исключения, если необходимо
				ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
			}
			await OnGetAsync();
		}
	}
}

