using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Collections;
using System.Linq;
using System.Text.Json;
using Marketplace_Web.Models;

namespace Marketplace_Web.Pages
{
    
		public class AdminProductsModel : PageModel
		{
			private readonly IHttpClientFactory _clientFactory;

			public AdminProductsModel(IHttpClientFactory clientFactory)
			{
				_clientFactory = clientFactory;
			}

			public List<User> Users { get; private set; }  = new List<User>();
			public List<Order> Orders { get; private set; }  = new List<Order>();
			public List<Category> Categories { get; private set; }  = new List<Category>();
			public List<OrderItem> OrderItems { get; private set; }  = new List<OrderItem>();
			public List<Review> Reviews { get; private set; }  = new List<Review>();
			public List<Role> Roles { get; private set; }  = new List<Role>();
			public List<Product> Products { get; private set; } = new List<Product>();
			
			public async Task OnGetAsync()
			{
				try
				{
					using (var httpClient = _clientFactory.CreateClient())
					{
						var apiUrl = "http://localhost:8080/api/product/getall"; // Замените на свой API-URL
						var response = await httpClient.GetAsync(apiUrl);

						if (response.IsSuccessStatusCode)
						{
							var itemsJson = await response.Content.ReadAsStringAsync();
							Products = JsonSerializer.Deserialize<List<Product>>(itemsJson);
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

