using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using Marketplace_Web.Models;

namespace Marketplace_Web.Pages
{
    public class MyProductsModel : PageModel
    {
		Marketplace1Context _context = new Marketplace1Context();
		public List<Product> Products { get; set; } = new List<Product>();
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
            Products = user.Products.ToList();

			return Page();
        }
        public async Task OnPostDeleteProductAsync(int productId)
        {
            ProductController productController = new ProductController(_context);
            var product = await productController.DeleteProduct(productId);
            await OnGetAsync();
		}

    }
}
