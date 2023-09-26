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
			// ����� �� ���������� GET-������ �� API, ����� �������� ���������� � ��������
			var apiUrl = $"http://localhost:8080/api/user/getbyid/{UserId}"; // �������� �� ���� URL
			var client = new HttpClient();

			var response = await client.GetAsync(apiUrl);

			if (response.IsSuccessStatusCode)
			{
				var productJson = await response.Content.ReadAsStringAsync();
				User user = JsonSerializer.Deserialize<User>(productJson);
			}
			else
			{
				return NotFound(); // ���� ������� �� ������
			}


			return Page();
		}
	}
    }

