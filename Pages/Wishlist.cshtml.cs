using Marketplace_Web.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace Marketplace_Web.Pages
{
    public class WishlistModel : PageModel
    {
        public List<Wishlist>? Wishlist { get; set; } = new List<Wishlist>();
        public List<Product>? Products { get; set; } = new List<Product>();

        public async Task<IActionResult> OnGetAsync()
        {
			var action = Request.Query["action"].ToString();
			if (action == "load")
				using (var httpClient = new HttpClient())
            {
                var apiUrl = "http://localhost:8080/api/wishlist/getbyfields";
                var userId = HttpContext.Session.GetInt32("UserId");

                // Подготовьте JSON-запрос для API
                var requestData = new
                {
                    UserID = userId
                };

                var jsonRequestData = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                { 
                    var wishlistJson = await response.Content.ReadAsStringAsync();
                    Wishlist = JsonSerializer.Deserialize<List<Wishlist>>(wishlistJson);
                    foreach (var wishlist in Wishlist)
                    {
                        var productId = wishlist.ProductId;
                        apiUrl = $"http://localhost:8080/api/product/getbyid/{productId}";
                        response = await httpClient.GetAsync(apiUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var productJson = await response.Content.ReadAsStringAsync();
                            Products.Add(JsonSerializer.Deserialize<Product>(productJson));
                        }
                    }
                }
                else
                {
                    // Обработка ошибки
                    // Установите сообщение об ошибке в вашей модели
                }
            }

            return Page();
        }

    }
}
