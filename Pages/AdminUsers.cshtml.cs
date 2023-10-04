using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

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

		public async Task<IActionResult> OnGetAsync()
		{
			if (HttpContext.Session.GetInt32("RoleId") < 2 || HttpContext.Session.GetInt32("RoleId") == null)
				return RedirectToPage("/Index");
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
			return Page();
		}
		[BindProperty]
		public int ItemId { get; set; }

		public string DeleteResult { get; set; }

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					// Замените apiUrl на URL вашего API для удаления элемента
					var apiUrl = $"http://localhost:8080/api/user/deletebyid/{ItemId}";
					var response = await httpClient.DeleteAsync(apiUrl);

					if (response.IsSuccessStatusCode)
					{
						DeleteResult = "Item successfully deleted.";
					}
					else
					{
						DeleteResult = "Failed to delete item. Please check the ID.";
					}
				}
			}
			catch (Exception ex)
			{
				DeleteResult = $"An error occurred: {ex.Message}";
			}
			await OnGetAsync();
			return Page();
		}
	}
}

