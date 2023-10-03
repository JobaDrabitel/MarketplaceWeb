using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Marketplace_Web.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using System.Net;

namespace Marketplace_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public static  User user;
		private readonly IMemoryCache _cache;
		public static  IEnumerable<Category> Categories { get; private set; }
		public static  IEnumerable<Product> Products { get; private set; }
		public int Rating { get; private set; }
		public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
			user =  UserSessions.GetUser(HttpContext.Session);
			Categories = await GetCategories();
			Products = await GetProducts();
		}
        private async Task<IEnumerable<Category>> GetCategories()
        {
			var categoriesJson = HttpContext.Session.GetString("Categories");

			if (string.IsNullOrEmpty(categoriesJson))
				using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync("http://localhost:8080/api/category/getall");
					if (response.IsSuccessStatusCode)
					{
						categoriesJson = await response.Content.ReadAsStringAsync();

						// Сохраните категории в сессии для последующего использования
						HttpContext.Session.SetString("Categories", categoriesJson);
					}

				}
			var categories = JsonSerializer.Deserialize<IEnumerable<Category>>(categoriesJson);
			return categories;
		}
		public async Task<IEnumerable<Product>> GetProducts()
		{
			using (var httpClient = new HttpClient())
			{
				var apiUrl = "http://localhost:8080/api/product/getall";
				var response = await httpClient.GetAsync(apiUrl);
				var productsJson = await response.Content.ReadAsStringAsync();
				var products = JsonSerializer.Deserialize<List<Product>>(productsJson);
				return products;
			}
		}
		public async Task<IActionResult> OnPostAddToCartAsync(int productId)
		{
			var cart = _cache.Get<Dictionary<int, int>>("Cart") ?? new Dictionary<int, int>();
			if (cart.ContainsKey(productId))
			{
				cart[productId]++;
			}
			else
			{
				cart[productId] = 1;
			}
			_cache.Set("Cart", cart);
			return RedirectToPage("/Index"); 
		}
		public IActionResult OnGetLogout(string returnUrl)
		{
			// Очищаем сессию и устанавливаем isAuthenticated в false
			HttpContext.Session.Clear();
			user = null;

			// Проверяем, был ли предоставлен returnUrl, и перенаправляем на него, если да.
			if (!string.IsNullOrEmpty(returnUrl))
			{
				return Redirect(returnUrl);
			}

			// Если returnUrl не был предоставлен, перенаправляем пользователя на другую страницу после выхода.
			return RedirectToPage("/index"); // Замените 'ДругаяСтраница' на нужный URL
		}
		[HttpPost]
		public IActionResult OnPostSubscribe(string email)
		{
			// Здесь вызывайте ваш метод SendMessageAsync() и передавайте в него email
			 SendMessageAsync(email);

			// Возвращайте представление или редирект, в зависимости от вашей логики
			return Page();
		}
		static async Task SendMessageAsync(string email)
		{
			string fromEmail = "legenadary.pigeon@gmail.com"; // Ваш адрес электронной почты Gmail
			string fromEmailPassword = "burz uiew yaqc zfrq"; // Пароль от вашей почты Gmail
			string toEmail = email; // Адрес получателя

			SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential(fromEmail, fromEmailPassword),
				EnableSsl = true,
			};

			MailMessage mailMessage = new MailMessage
			{
				From = new MailAddress(fromEmail),
				Subject = "Subscribe",
				Body = "I am subscribe and you have been subscribed!",
			};

			mailMessage.To.Add(toEmail);

			try
			{
				smtpClient.Send(mailMessage);
				Console.WriteLine("Письмо успешно отправлено.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при отправке письма: {ex.Message}");
			}
		}
	}
}