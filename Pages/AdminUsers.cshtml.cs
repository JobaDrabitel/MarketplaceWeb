using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Marketplace_Web.Pages
{
    public class AdminUsersModel : PageModel
    {
		Marketplace1Context _context = new Marketplace1Context();
		public List<User> Users { get; private set; } = new List<User>();

		public async Task<IActionResult> OnGetAsync()
		{
			if (HttpContext.Session.GetInt32("RoleId") < 2 || HttpContext.Session.GetInt32("RoleId") == null)
				return RedirectToPage("/Index");
			try
			{

				Users = await _context.Users.ToListAsync();
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
			UserController userController = new UserController(_context);
			try
			{
				var review = await userController.DeleteUser(ItemId);
				await _context.SaveChangesAsync();
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

