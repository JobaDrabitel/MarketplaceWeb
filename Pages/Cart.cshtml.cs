using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using Marketplace_Web.Models;

namespace Marketplace_Web.Pages
{
    public class WishlistModel : PageModel
    {
		public bool OrderSuccess { get; set; }
		Marketplace1Context _context = new Marketplace1Context();
		public List<Product>? Products { get; set; } = new List<Product>();

        public async Task<IActionResult> OnGetAsync()
        {
			var currentRole = HttpContext.Session.GetInt32("RoleId");
			var user = UserSessions.GetUser(HttpContext.Session);
			if (user != null && currentRole == 2)
				return RedirectToPage("/Moderator");
			else if (user != null && currentRole == 3)
				return RedirectToPage("/Admin");
			else if (user != null && currentRole == 4)
				return RedirectToPage("/Director");
			var userId = HttpContext.Session.GetInt32("UserId");
			if (userId != 0 && userId != null)
			{
				 user = await _context.Users.FindAsync(userId);
				if (user!=null)
                Products = user.ProductsNavigation.ToList();
			}
            return Page();
        }
		private async Task<List<Product>> GetSelectedProducts()
		{
			await OnGetAsync();
			return Products;
		}
		public async Task<IActionResult> OnPostPlaceOrderAsync(int productId, int count)
		{
			ProductController productController = new ProductController(_context);
			List<Product> selectedProducts = await GetSelectedProducts();
			if (selectedProducts == null)
				return Page();
			var product = await  productController.GetProduct(productId);

			if (product.StockQuantity < count)
			{
				ModelState.AddModelError(string.Empty, $"Произошла ошибка при оформлении заказа.");
				OrderSuccess = false;
				return Page();
			}
			try
			{
				Order order = new Order
				{
						UserId = HttpContext.Session.GetInt32("UserId"),
						CreateTime = DateTime.Now,
						TotalAmount = product.Price,
						ProductId = productId,
						TotalQuantity = count,
				};
				_context.Orders.Add(order);
				product.StockQuantity -= count;
				await productController.PutProduct(productId, product);
				await _context.SaveChangesAsync();
				await OnPostDeleteProductAsync(product.ProductId);
			}
			catch (Exception ex) 
			{
				ModelState.AddModelError(string.Empty, $"Произошла ошибка при оформлении заказа.");
				OrderSuccess = false;
			}
			OrderSuccess = true;
			return Page();
		}
		public async Task OnPostDeleteProductAsync(int productId)
		{

			var userId = HttpContext.Session.GetInt32("UserId");
			if (userId != 0 && userId != null)
			{
				var user = await _context.Users.FindAsync(userId);
				if (user != null)
				{
					var productToRemove = user.ProductsNavigation.FirstOrDefault(p => p.ProductId == productId);

					if (productToRemove != null)
					{
						user.ProductsNavigation.Remove(productToRemove);
						_context.SaveChanges(); // Сохраняем изменения в БД.
					}
				}
			}
			await OnGetAsync();
		}
	}
}

