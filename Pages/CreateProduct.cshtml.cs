using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketplace_Web {
	public class CreateProductModel : PageModel
	{
		[BindProperty]
		public string CategoryId { get; set; }
		Marketplace1Context _context = new Marketplace1Context();

		[BindProperty]
		public Product Product { get; set; }

		public IActionResult OnGet()
		{
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var category = await _context.Categories.FindAsync(Convert.ToInt32(CategoryId));
			if (!ModelState.IsValid)
			{
				return Page();
			}

			// Получите SellerUserId из сессии
			var userId = HttpContext.Session.GetInt32("UserId");
			var user = await _context.Users.FindAsync(userId);

			if (user == null || userId == 0)
			{
				// Пользователь не авторизован, выполните действия по вашему усмотрению
				return RedirectToPage("/Login"); // Например, перенаправьте на страницу входа
			}

			// Создайте экземпляр класса Product и заполните его данными из формы
			var product = new Product
			{
				Name = Product.Name,
				Description = Product.Description,
				Price = Product.Price,
				StockQuantity = Product.StockQuantity,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = null, // Вы можете установить значение по умолчанию
				ImageUrl = Product.ImageUrl,
				Categories = new List<Category>() { category }
			};
			// Добавьте продукт в коллекцию продуктов пользователя
			user.Products.Add(product);

			// Добавьте продукт в контекст базы данных
			_context.Products.Add(product);
			

			// Сохраните изменения в базе данных
			await _context.SaveChangesAsync();

			return RedirectToPage("MyProducts");
		}

	}
}
