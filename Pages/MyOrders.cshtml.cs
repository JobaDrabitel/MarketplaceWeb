using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using Marketplace_Web.Models;

namespace Marketplace_Web.Pages
{
	public class MyOrdersModel : PageModel
	{
		Marketplace1Context _context = new Marketplace1Context();
		public List<Order> Orders { get; set; } = new List<Order>();
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
			user = await _context.Users.FindAsync(userId);
			if (user == null) { RedirectToPage("/Login"); }
			Orders = user.Orders.ToList();
			Orders.RemoveAll(orders => orders.TotalQuantity < 1);
			return Page();
		}
		public async Task OnPostDeleteOrderAsync(int orderId)
		{
			OrderController orderController = new OrderController(_context);
			var order = await orderController.GetOrder(orderId);
			order.TotalQuantity = 0;
			await orderController.PutOrder(orderId, order);
			await _context.SaveChangesAsync();
			await OnGetAsync();
		}

	}
}
