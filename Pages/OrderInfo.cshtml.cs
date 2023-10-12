using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_Web.Pages
{
    public class OrderInfoModel : PageModel
    {
		Marketplace1Context _context = new Marketplace1Context();
		public Product Product { get; set; }
		public Order Order { get; set; }
		public User Seller { get; set; }
		public async Task<IActionResult> OnGetAsync(int orderId)
		{
			var userId = HttpContext.Session.GetInt32("UserId");
			var user = await _context.Users.FindAsync(userId);
			// Здесь мы отправляем GET-запрос на API, чтобы получить информацию о продукте
			Order = await _context.Orders.FindAsync(orderId);

			if (Order == null)
			{
				return NotFound(); // Если продукт не найден
			}
			try
			{
				Product = Order.Product;
				Seller = Product.Users.FirstOrDefault();
			}
			catch (Exception ex) { }
			return Page();


		}
	}
}
