using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketplace_Web.Pages
{
    public class AdminModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
			var currentRole = HttpContext.Session.GetInt32("RoleId");
			var user = UserSessions.GetUser(HttpContext.Session);
			if (user != null && currentRole == 2)
				return RedirectToPage("/Moderator");
			else if (user != null && currentRole == 4)
				return RedirectToPage("/Director");
			else if (user == null || currentRole == 1)
				return RedirectToPage("/Index");
			return Page();
		}
    }
}
