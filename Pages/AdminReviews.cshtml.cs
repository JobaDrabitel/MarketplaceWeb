using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Collections;
using System.Linq;
using System.Text.Json;
using Marketplace_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_Web.Pages
{

	public class AdminReviewsModel : PageModel
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
				ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
			}
			return Page();
		}
		[BindProperty]
		public int ItemId { get; set; }

		public string DeleteResult { get; set; }

		public async Task<IActionResult> OnPostAsync()
		{
			ReviewController reviewController = new ReviewController(_context);
			try
			{
				var review = reviewController.DeleteReview(ItemId);
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

