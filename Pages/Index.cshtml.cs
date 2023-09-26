using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Marketplace_Web.Pages.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace Marketplace_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public User user;

		public IEnumerable<Category> Categories { get; private set; }
		public IEnumerable<Product> Products { get; private set; }
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

	}
}