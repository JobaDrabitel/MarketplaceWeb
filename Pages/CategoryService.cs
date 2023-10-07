using API_Marketplace_.net_7_v1.Models;
using System.Text.Json;
using System.Net.Http;

namespace Marketplace_Web.Pages
{
	public class CategoryService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CategoryService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
		{
			_httpClientFactory = httpClientFactory;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
		{
			// Попробуйте сначала получить категории из сессии
			var session = _httpContextAccessor.HttpContext.Session;
			var categoriesJson = session.GetString("Categories");

			if (string.IsNullOrEmpty(categoriesJson))
			{
				// Если категории не были найдены в сессии, выполните запрос к API
				var client = _httpClientFactory.CreateClient();
				var response = await client.GetAsync("http://localhost:8080/api/category/getall");

				if (response.IsSuccessStatusCode)
				{
					categoriesJson = await response.Content.ReadAsStringAsync();

					// Сохраните категории в сессии для последующего использования
					session.SetString("Categories", categoriesJson);
				}
			}

			// Десериализуйте категории и верните их
			var categories = JsonSerializer.Deserialize<IEnumerable<Category>>(categoriesJson);
			return categories;
		}
	}

}
