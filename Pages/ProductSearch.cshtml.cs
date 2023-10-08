using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Marketplace_Web.Models;

namespace Marketplace_Web.Pages
{
    public class ProductSearchModel : PageModel
    {
		Marketplace1Context _context = new Marketplace1Context();
		public int CategoryId { get; set; }
        public List<Product>? Products { get; set; } = new List <Product>();
        public async Task<IActionResult> OnPostAsync(string searchTerm, int categoryId)
        {
			var currentRole = HttpContext.Session.GetInt32("RoleId");
			var user = UserSessions.GetUser(HttpContext.Session);
			if (user != null && currentRole == 2)
				return RedirectToPage("/Moderator");
			else if (user != null && currentRole == 3)
				return RedirectToPage("/Admin");
			else if (user != null && currentRole == 4)
				return RedirectToPage("/Director");
			CategoryId = categoryId;
                    var products = await _context.Products.ToListAsync();
					if (products != null)
						if (String.IsNullOrEmpty(searchTerm) && categoryId != 0)
							foreach (var product in products)
							{
								if (product.Categories.First().CategoryId == categoryId)
									Products.Add(product);
							}
						else if (categoryId == 0 && !String.IsNullOrEmpty(searchTerm))
							foreach (var product in products)
							{
								if (product.Name.ToUpper().Contains(searchTerm.ToUpper()))
									Products.Add(product);
							}
						else if (!String.IsNullOrEmpty(searchTerm) && categoryId != 0)
							foreach (var product in products)
							{
								if (product.Name.ToUpper().Contains(searchTerm.ToUpper()) && product.Categories.First().CategoryId == categoryId)
									Products.Add(product);
							}
						else
							Products = products;
					Products.RemoveAll(product => product.UpdatedAt == null || product.StockQuantity == 0);
				
            return Page();
               
            

        }
		public async Task<IActionResult> OnGet(int Category)
		{
			CategoryId = Category;
			var products = await _context.Products.ToListAsync();
			foreach (var product in products)
			{
				if (product.Categories.First().CategoryId == CategoryId)
					Products.Add(product);
			}
			return Page();
		}

	}
}
