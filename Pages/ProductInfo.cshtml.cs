using Marketplace_Web.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace Marketplace_Web.Pages
{
	public class ProductInfoModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public User user;
        public ProductInfoModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public Product Product { get; set; }
        public List<Review> Reviews { get; set; }
        public string Category { get; set; }
        public User Seller { get; set; }
        public List<User> Users { get; set; }

        public async Task<IActionResult> OnGetAsync(int productId)
        {
			user = UserSessions.GetUser(HttpContext.Session);
			Users = new List<User>();
            // Здесь мы отправляем GET-запрос на API, чтобы получить информацию о продукте
            var apiUrl = $"http://localhost:8080/api/product/getbyid/{productId}"; // Замените на свой URL
            var client = _clientFactory.CreateClient();

            var response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var productJson = await response.Content.ReadAsStringAsync();
                Product = JsonSerializer.Deserialize<Product>(productJson);
            }
            else
            {
                return NotFound(); // Если продукт не найден
            }

            var categoryRespose = await client.GetAsync($"http://localhost:8080/api/category/getbyid/{Product.CategoryId}");
            var categoryJson = await categoryRespose.Content.ReadAsStringAsync();
            try
            {
                Category = JsonSerializer.Deserialize<Category>(categoryJson).Name;


            
            var sellerResponse = await client.GetAsync($"http://localhost:8080/api/user/getbyid/{Product.SellerUserId}");
            var sellerJson = await sellerResponse.Content.ReadAsStringAsync();
            Seller = JsonSerializer.Deserialize<User>(sellerJson); }
			catch (Exception ex) { Category = "Нет"; Seller  = null; }
			// Теперь отправляем POST-запрос для получения отзывов
			var reviewRequest = new { ProductId = productId };
            var reviewRequestJson = JsonSerializer.Serialize(reviewRequest);

            var reviewResponse = await client.PostAsync($"http://localhost:8080/api/review/getbyfields", new StringContent(reviewRequestJson, System.Text.Encoding.Default, "application/json")); // Замените на свой URL
            if (reviewResponse.IsSuccessStatusCode)
            {
                var reviewJson = await reviewResponse.Content.ReadAsStringAsync();
                Reviews = JsonSerializer.Deserialize<List<Review>>(reviewJson);
            }
            else
            {
                Reviews = new List<Review>();
            }
            foreach (var review in Reviews)
            {
                var userResponse = await client.GetAsync($"http://localhost:8080/api/user/getbyid/{review.UserId}");
                var userJson = await userResponse.Content.ReadAsStringAsync();
                Users.Add(JsonSerializer.Deserialize<User>(userJson));
            }

            return this.Page();

        }
        public async Task<IActionResult> OnPostAsync(int productId, int rating, string comment, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/ProductInfo", new { productId });
            }

         
            using (var httpClient = new HttpClient())
            {
                var reviewData = new
                {
                    UserId = HttpContext.Session.GetInt32("UserId"),
                    ProductId = productId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow,
                    ImageURL = ""
                };
                var jsonData = JsonSerializer.Serialize(reviewData);
                // Замените URL на ваше API-URL
                var apiUrl = "http://localhost:8080/api/review/create";

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/ProductInfo", new { productId });
                }
                else
                {
                    // Обработка ошибки
                    ModelState.AddModelError(string.Empty, "Failed to create review.");
                    return RedirectToPage("/ProductInfo", new { productId });
                }
            }
        }

    }


}
