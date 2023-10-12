using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketplace_Web.Pages
{
    public class UserInfoModel : PageModel
    {
		Marketplace1Context _context = new Marketplace1Context();
		public List<Product> Products { get; set; } = new List<Product>();
		public User User { get; set; }
		public async Task<IActionResult> OnGetAsync(int userId)
		{
			User = await _context.Users.FindAsync(userId);
			if (User == null)
			{
				return NotFound(); 
			}
			try
			{
				Products = User.Products.ToList();
			}
			catch (Exception ex) { await OnGetAsync(userId); }
			return Page();


		}
	}
}
