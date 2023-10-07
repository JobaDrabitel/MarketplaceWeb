using API_Marketplace_.net_7_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace Marketplace_Web.Pages
{
    public class MyProductsModel : PageModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public async Task<IActionResult> OnGetAsync()
        {
            // Получите текущего пользователя из сессии (псевдокод, вам нужно реализовать получение из сессии)
            var userId = HttpContext.Session.GetInt32("UserId");

            // Подготовьте JSON-запрос для API
            var requestData = new
            {
                SellerUserId = userId
            };

            var jsonRequestData = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

            // Замените URL на ваше API-URL
            var apiUrl = "http://localhost:8080/api/product/getbyfields";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<Product>>(jsonResponse);
					products.RemoveAll(product => product.UpdatedAt == null || product.StockQuantity == 0);
                    Products = products;
				}
                else
                {
                    throw new Exception();
                }
            }

            return Page();
        }
        public async Task OnPostDeleteProductAsync(int productId)
        {
			var apiUrl = $"http://localhost:8080/api/product/delete/{productId}";
			using (var httpClient = new HttpClient())
			{
				var response = await httpClient.GetAsync(apiUrl);     
			}
            await OnGetAsync();
		}

    }
}
