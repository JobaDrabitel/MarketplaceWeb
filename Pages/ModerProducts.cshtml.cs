using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Collections;
using System.Linq;
using System.Text.Json;
using API_Marketplace_.net_7_v1.Models;

namespace Marketplace_Web.Pages
{
    
		public class ModerProductsModel : PageModel
		{
			private readonly IHttpClientFactory _clientFactory;

			public ModerProductsModel(IHttpClientFactory clientFactory)
			{
				_clientFactory = clientFactory;
			}

			public List<User> Users { get; private set; }  = new List<User>();
			public List<Order> Orders { get; private set; }  = new List<Order>();
			public List<Category> Categories { get; private set; }  = new List<Category>();
			public List<Review> Reviews { get; private set; }  = new List<Review>();
			public List<Role> Roles { get; private set; }  = new List<Role>();
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
							Products.RemoveAll(product => product.UpdatedAt != null || product.StockQuantity == 0);
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
		}
	}

