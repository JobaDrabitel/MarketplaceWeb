using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Collections;
using System.Linq;
using System.Text.Json;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using Marketplace_Web.Models;

namespace Marketplace_Web.Pages
{

	public class ModerReviewsModel : PageModel
	{
		Marketplace1Context _context = new Marketplace1Context();


		public List<Review> Reviews { get; private set; } = new List<Review>();

		public async Task<IActionResult> OnGetAsync()
		{
			if (HttpContext.Session.GetInt32("RoleId") < 2 || HttpContext.Session.GetInt32("RoleId") == null)
				return RedirectToPage("/Index");
			try
			{

				Reviews = await _context.Reviews.ToListAsync();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"Îøèáêà: {ex.Message}");
			}
			return Page();
		}
	}
}

