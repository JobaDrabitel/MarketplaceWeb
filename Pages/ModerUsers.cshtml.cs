using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Collections;
using System.Linq;
using System.Text.Json;
using Marketplace_Web.Models;
using System.Reflection.Metadata.Ecma335;

namespace Marketplace_Web.Pages
{

	public class ModerUsersModel : PageModel
	{
		private readonly Marketplace1Context dbContext = new Marketplace1Context();

		public ModerUsersModel()
		{
		}

		public List<User> Users { get; private set; } = new List<User>();

		public async Task<IActionResult> OnGetAsync()
		{
			if (HttpContext.Session.GetInt32("RoleId") < 2 || HttpContext.Session.GetInt32("RoleId") == null)
				return RedirectToPage("/Index");
			try
			{
				var userContoller = new UserController(dbContext);

				Users = await userContoller.GetUsers() ; // Замените на свой API-URL

						foreach (var user in Users)
							if (user.Roles.First().RoleId!= 1) { Users.Remove(user); }
					
					else
					{
						// Обработайте ошибку, если не удалось получить данные
						ModelState.AddModelError(string.Empty, "Произошла ошибка при получении данных.");
					}
				
			}
			catch (Exception ex)
			{
				// Обработайте другие исключения, если необходимо
				ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
			}
			return Page();
		}
		
	}
}

