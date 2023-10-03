using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace Marketplace_Web.Pages
{
	public class MyOrdersModel : PageModel
	{
		public ICollection<Order> Orders { get; set; }
		public ICollection<Product> Products { get; set; }
		public ICollection<OrderItem> OrderItems { get; set; }
		public async Task<IActionResult> OnGetAsync()
		{
			// �������� �������� ������������ �� ������ (���������, ��� ����� ����������� ��������� �� ������)
			var userId = HttpContext.Session.GetInt32("UserId");

			// ����������� JSON-������ ��� API
			var requestData = new
			{
				UserId = userId
			};

			var jsonRequestData = JsonSerializer.Serialize(requestData);
			var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

			// �������� URL �� ���� API-URL
			var apiUrl = "http://localhost:8080/api/order/getbyfields";

			using (var httpClient = new HttpClient())
			{
				var response = await httpClient.PostAsync(apiUrl, content);

				if (response.IsSuccessStatusCode)
				{
					var jsonResponse = await response.Content.ReadAsStringAsync();
					var orders = JsonSerializer.Deserialize<List<Order>>(jsonResponse);
					Orders = orders;
					apiUrl = $"http://localhost:8080/api/order/getbyfields";
					foreach (var order in Orders)
					{
					}
				}
				else
				{
				}
			}

			return Page();
		}

	}
}
