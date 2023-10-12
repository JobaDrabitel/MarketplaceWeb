using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_Web.Pages
{
	public class ModeratorCancelledOrdersModel : PageModel
	{
		public string ApproveResult { get; set; }

		public List<Order> Orders { get; private set; } = new List<Order>();

		Marketplace1Context _context = new Marketplace1Context();
		public async Task<IActionResult> OnGetAsync()
		{
			if (HttpContext.Session.GetInt32("RoleId") < 2 || HttpContext.Session.GetInt32("RoleId") == null)
				return RedirectToPage("/Index");
			try
			{

				Orders = await _context.Orders.ToListAsync();
				Orders.RemoveAll(order => order.TotalQuantity != 0);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
			}
			return Page();
		}
		public async Task OnPostApproveProductAsync(string id)
		{
			OrderController orderController = new OrderController(_context);
			try
			{
				var orderId = Convert.ToInt32(id);
				var order = await orderController.GetOrder(orderId);
				order.TotalQuantity = 1;
				order = await orderController.PutOrder(orderId, order);
				await _context.SaveChangesAsync();

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
			OrderController orderController = new OrderController(_context);
			try
			{
				var orderId = Convert.ToInt32(id);
				var order = await orderController.DeleteOrder(orderId);
				await _context.SaveChangesAsync();
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

