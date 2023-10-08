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

				Users = await userContoller.GetUsers() ; // �������� �� ���� API-URL

						foreach (var user in Users)
							if (user.Roles.First().RoleId!= 1) { Users.Remove(user); }
					
					else
					{
						// ����������� ������, ���� �� ������� �������� ������
						ModelState.AddModelError(string.Empty, "��������� ������ ��� ��������� ������.");
					}
				
			}
			catch (Exception ex)
			{
				// ����������� ������ ����������, ���� ����������
				ModelState.AddModelError(string.Empty, $"������: {ex.Message}");
			}
			return Page();
		}
		
	}
}

