using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Collections;
using System.Linq;
using System.Text.Json;
using Marketplace_Web.Models;

namespace Marketplace_Web.Pages
{

	public class AdminUsersModel : PageModel
	{
		private readonly IHttpClientFactory _clientFactory;

		public AdminUsersModel(IHttpClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}

		public List<User> Users { get; private set; } = new List<User>();

		public async Task OnGetAsync()
		{
			try
			{
				using (var httpClient = _clientFactory.CreateClient())
				{
					var apiUrl = "http://localhost:8080/api/user/getall"; // Замените на свой API-URL
					var response = await httpClient.GetAsync(apiUrl);

					if (response.IsSuccessStatusCode)
					{
						var itemsJson = await response.Content.ReadAsStringAsync();
						Users = JsonSerializer.Deserialize<List<User>>(itemsJson);
					}
					else
					{
						// Обработайте ошибку, если не удалось получить данные
						ModelState.AddModelError(string.Empty, "Произошла ошибка при получении данных.");
					}
				}
			}
			catch (Exception ex)
			{
				// Обработайте другие исключения, если необходимо
				ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
			}
		}
	}
}

