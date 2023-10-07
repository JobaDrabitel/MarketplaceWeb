using API_Marketplace_.net_7_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace Marketplace_Web.Pages
{
    public class ModeratorApproveProductsModel : PageModel
    {
		public string ApproveResult { get; set; }	
		private readonly IHttpClientFactory _clientFactory;

		public ModeratorApproveProductsModel(IHttpClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}

		public List<Product> Products { get; private set; } = new List<Product>();


		public async Task<IActionResult> OnGetAsync()
		{
			if (HttpContext.Session.GetInt32("RoleId") < 2 || HttpContext.Session.GetInt32("RoleId") == null)
				return RedirectToPage("/Index");
			try
			{
				using (var httpClient = _clientFactory.CreateClient())
				{
					var apiUrl = "http://localhost:8080/api/product/getall"; // �������� �� ���� API-URL
					var response = await httpClient.GetAsync(apiUrl);

					if (response.IsSuccessStatusCode)
					{
						var itemsJson = await response.Content.ReadAsStringAsync();
						Products = JsonSerializer.Deserialize<List<Product>>(itemsJson);
						Products.RemoveAll(product => product.UpdatedAt != null);

					}
					else
					{
						// ����������� ������, ���� �� ������� �������� ������
						ModelState.AddModelError(string.Empty, "��������� ������ ��� ��������� ������.");
					}
				}
			}
			catch (Exception ex)
			{
				// ����������� ������ ����������, ���� ����������
				ModelState.AddModelError(string.Empty, $"������: {ex.Message}");
			}
			return Page();
		}
		public async Task OnPostApproveProductAsync(string id)
		{
			try
			{
				using (var httpClient = _clientFactory.CreateClient())
				{
					var loginData = new
					{
						UpdatedAt = DateTime.Now
					};

					// ������������ ������ � JSON
					var jsonData = JsonSerializer.Serialize(loginData);
					var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
					var apiUrl = $"http://localhost:8080/api/product/updatebyid/{id}"; // �������� �� ���� API-URL
					var response = await httpClient.PostAsync(apiUrl, content);

					if (response.IsSuccessStatusCode)
					{
						ApproveResult = "������� ������� �����������!";
					}
					else
					{
						// ����������� ������, ���� �� ������� �������� ������
						ApproveResult = "������� ������� �����������!";
					}
				}
			}
			catch (Exception ex)
			{
				// ����������� ������ ����������, ���� ����������
				ModelState.AddModelError(string.Empty, $"������: {ex.Message}");
			}
				await OnGetAsync();
		}
	}
}

