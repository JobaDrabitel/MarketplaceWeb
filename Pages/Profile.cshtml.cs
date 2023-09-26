using Marketplace_Web.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Marketplace_Web.Pages
{
    public class ProfileModel : PageModel
    {
		public async Task<IActionResult> OnGetAsync(int UserId)
		{
			// Здесь мы отправляем GET-запрос на API, чтобы получить информацию о продукте
			var apiUrl = $"http://localhost:8080/api/user/getbyid/{UserId}"; // Замените на свой URL
			var client = new HttpClient();

			var response = await client.GetAsync(apiUrl);

			if (response.IsSuccessStatusCode)
			{
				var productJson = await response.Content.ReadAsStringAsync();
				User user = JsonSerializer.Deserialize<User>(productJson);
			}
			else
			{
				return NotFound(); // Если продукт не найден
			}


			return Page();
		}
	}
    }

