using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Marketplace_Web.Pages
{
	public class ProductInfoModel : PageModel
	{
		public User user;
		Marketplace1Context _context = new Marketplace1Context();
		public Product Product { get; set; }
		public List<Review> Reviews { get; set; } = new List<Review>();
		public string Category { get; set; }
		public User Seller { get; set; }
		public List<User> Users { get; set; } = new List<User>();
		public List<Product> Advices { get; set; }
		public bool IsAdded { get; set; } = false;

		public async Task<IActionResult> OnGetAsync(int productId)
		{
			var userId = HttpContext.Session.GetInt32("UserId");
			var user = await _context.Users.FindAsync(userId);
			// Здесь мы отправляем GET-запрос на API, чтобы получить информацию о продукте
			Product = await _context.Products.FindAsync(productId);

			if (Product == null)
			{
				return NotFound(); // Если продукт не найден
			}
			try
			{
				Category = Product.Categories.FirstOrDefault<Category>().Name;
				Seller = Product.Users.First();
			}
			catch (Exception ex) { Category = "Нет"; Seller = null; }
			Reviews = Product.Reviews.ToList();

			foreach (var review in Reviews)
				Users.Add(review.User);
			await GetAdvices(Product.Categories.First().CategoryId);
			return Page();


		}
		public async Task<IActionResult> OnPostAsync(int productId, int rating, string comment, string? image)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToPage("/ProductInfo", new { productId });
			}



			Review review = new Review()
			{
				UserId = HttpContext.Session.GetInt32("UserId"),
				ProductId = productId,
				Rating = rating,
				Comment = comment,
				CreatedAt = DateTime.UtcNow,
				ImageUrl = image
			};
			try
			{
				_context.Reviews.Add(review);
				await _context.SaveChangesAsync();
			}


			catch
			{
				// Обработка ошибки
				ModelState.AddModelError(string.Empty, "Failed to create review.");
			}
				return RedirectToPage("/ProductInfo", new { productId });

		}
		private async Task GetAdvices(int? categoryId) =>
			Advices = await _context.Products
			.Where(p => p.Categories.Any(c => c.CategoryId == categoryId))
			.Take(5)
			.ToListAsync();
		public async Task<IActionResult> OnPostAddToCartAsync(int productId)
		{

			var userId = HttpContext.Session.GetInt32("UserId");
			var user = await _context.Users.FindAsync(userId);
			if (user == null)
				return RedirectToPage("/Login");
			var product = await _context.Products.FindAsync(productId);
			user.ProductsNavigation.Add(product);
			product.UsersNavigation.Add(user);
			await _context.SaveChangesAsync();
			return RedirectToPage("/ProductInfo", new { productId });
		}


	}
}
