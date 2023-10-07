using API_Marketplace_.net_7_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.Net.Http;

namespace Marketplace_Web.Pages
{
    public class WishlistModel : PageModel
    {
        public List<Product>? Products { get; set; } = new List<Product>();

        public async Task<IActionResult> OnGetAsync()
        {
			var action = Request.Query["action"].ToString();
			using (var httpClient = new HttpClient())
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var apiUrl = $"http://localhost:8080/api/productgetbyuser/{userId}";
				if (userId == null)
					return Page();
               
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                { 
                    var wishlistJson = await response.Content.ReadAsStringAsync();
                    Products = JsonSerializer.Deserialize<List<Product>>(wishlistJson);
                }
                else
                {
                    // Обработка ошибки
                    // Установите сообщение об ошибке в вашей модели
                }
            }

            return Page();
        }
		private async Task<List<Product>> GetSelectedProducts()
		{
			await OnGetAsync();
			return Products;
		}
		public async Task<IActionResult> OnPostPlaceOrderAsync()
		{
			// Получите данные из поля Wishlist или любого другого места, где хранятся выбранные товары
			List<Product> selectedProducts = await GetSelectedProducts();
			decimal? sum = 0;
			foreach (var product in selectedProducts)
			{
				if (product.StockQuantity < 1)
				{
					ModelState.AddModelError(string.Empty, $"Произошла ошибка при оформлении заказа. Товара {product.Name}  нет в наличии");
					return Page();
				}
				sum += product.Price;
			}
			if (sum == 0)
			{
				return RedirectToPage("/Index");
			}

			

			using (var httpClient = new HttpClient())
			{
				foreach (var product in selectedProducts)
				{
					Order order = new Order
					{
						UserId = HttpContext.Session.GetInt32("UserId"),
						CreateTime = DateTime.Now,
						TotalAmount = sum,
						ProductId = product.ProductId,
					};

					// Отправьте заказ и элементы заказа на ваше API
					var apiUrl = "http://localhost:8080/api/order/create";
					var jsonData = JsonSerializer.Serialize(order);
					var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
					var response = await httpClient.PostAsync(apiUrl, content);
					await OnPostDeleteProductAsync(product.ProductId);
				}
			}
			return Page();
		}
		public async Task OnPostDeleteProductAsync(int productId)
		{
			using (var httpClient = new HttpClient())
			{
				var userId = HttpContext.Session.GetInt32("UserId");
				var apiUrl = $"http://localhost:8080/api/user/getbyid/{userId}";
				var response = await httpClient.GetAsync(apiUrl);
				var User = await response.Content.ReadFromJsonAsync<User>();
			}
			await OnGetAsync();
		}
	}
	}

