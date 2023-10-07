using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API_Marketplace_.net_7_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketplace_Web {
	public class CreateProductModel : PageModel
	{
		private readonly IHttpClientFactory _clientFactory;

		public CreateProductModel(IHttpClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}

		[BindProperty]
		public Product Product { get; set; }

		public IActionResult OnGet()
		{
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			// Получите SellerUserId из сессии
			var sellerUser = UserSessions.GetUser(HttpContext.Session);

			if (sellerUser == null)
			{
				// Пользователь не авторизован, выполните действия по вашему усмотрению
				return RedirectToPage("/Login"); // Например, перенаправьте на страницу входа
			}

			// Создайте объект для отправки на API
			Product productData = new Product
			{
				Name = Product.Name,
				Description = Product.Description,
				Categories = Product.Categories,
				Price = Product.Price,
				ImageUrl = Product.ImageUrl,
				StockQuantity = Product.StockQuantity,
				UsersNavigation = new List<User> { sellerUser },
				CreatedAt = DateTime.Now,
			};

			// Сериализуйте данные в JSON
			var jsonData = JsonSerializer.Serialize(productData);

			// Отправьте данные на API
			using (var httpClient = _clientFactory.CreateClient())
			{
				var apiUrl = "http://localhost:8080/api/product/create";
				var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
				var response = await httpClient.PutAsync(apiUrl, content);

				if (response.IsSuccessStatusCode)
				{
					// Товар успешно создан, выполните необходимые действия, например, перенаправьтесь на страницу с подтверждением
					return RedirectToPage("/MyProducts");
				}
				else
				{
					// Обработайте ошибку, например, показав сообщение об ошибке пользователю
					ModelState.AddModelError(string.Empty, "Произошла ошибка при создании товара.");
					return Page();
				}
			}
		}
	}
}
