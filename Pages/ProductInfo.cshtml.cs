using Marketplace_Web.Models;
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
        public List<Product> Advices { get; set; }
        public bool IsAdded { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(int productId)
        {
            user = UserSessions.GetUser(HttpContext.Session);
            Users = new List<User>();
			// ����� �� ���������� GET-������ �� API, ����� �������� ���������� � ��������
			var apiUrl = $"http://localhost:8080/api/product/getbyid/{productId}"; // �������� �� ���� URL
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var productJson = await response.Content.ReadAsStringAsync();
                    Product = JsonSerializer.Deserialize<Product>(productJson);
                }
                else
                {
                    return NotFound(); // ���� ������� �� ������
                }

                var categoryRespose = await client.GetAsync($"http://localhost:8080/api/category/getbyid/{Product.CategoryId}");
                var categoryJson = await categoryRespose.Content.ReadAsStringAsync();
                try
                {
                    Category = JsonSerializer.Deserialize<Category>(categoryJson).Name;
                    var sellerResponse = await client.GetAsync($"http://localhost:8080/api/user/getbyid/{Product.SellerUserId}");
                    var sellerJson = await sellerResponse.Content.ReadAsStringAsync();
                    Seller = JsonSerializer.Deserialize<User>(sellerJson);
                }
                catch (Exception ex) { Category = "���"; Seller = null; }
                // ������ ���������� POST-������ ��� ��������� �������
                var reviewRequest = new { ProductId = productId };
                var reviewRequestJson = JsonSerializer.Serialize(reviewRequest);

                var reviewResponse = await client.PostAsync($"http://localhost:8080/api/review/getbyfields", new StringContent(reviewRequestJson, System.Text.Encoding.Default, "application/json")); // �������� �� ���� URL
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
                    if (review.UserId == null)
                        review.UserId = 2;
                    var userResponse = await client.GetAsync($"http://localhost:8080/api/user/getbyid/{review.UserId}");
                    var userJson = await userResponse.Content.ReadAsStringAsync();
                    Users.Add(JsonSerializer.Deserialize<User>(userJson));
                }
                await GetAdvices(Product.CategoryId);
                return this.Page();
            }

        }
        public async Task<IActionResult> OnPostAsync(int productId, int rating, string comment, string? image)
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
                    ImageUrl = image
                };
                var jsonData = JsonSerializer.Serialize(reviewData);
                // �������� URL �� ���� API-URL
                var apiUrl = "http://localhost:8080/api/review/create";

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/ProductInfo", new { productId });
                }
                else
                {
                    // ��������� ������
                    ModelState.AddModelError(string.Empty, "Failed to create review.");
                    return RedirectToPage("/ProductInfo", new { productId });
                }
            }
        }
        private async Task GetAdvices(int? categoryId)
        {
            var requestData = new
            {
                CategoryId = categoryId
            };

            var jsonRequestData = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

            // �������� URL �� ���� API-URL
            var apiUrl = "http://localhost:8080/api/product/getbyfields";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<Product>>(jsonResponse);
                    Advices = products;
                }
                else
                {
                    throw new Exception();
                }
            }

        }
		public async Task<IActionResult> OnPostAddToCartAsync(int productId)
		{
          
			var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var regData = new
                {
                    ProductId = productId,
                    UserId = userId
                };


                var jsonData = JsonSerializer.Serialize(regData);

                using (var httpClient = new HttpClient())
                {
                    // �������� URL �� ���� API-URL
                    var apiUrl = "http://localhost:8080/api/wishlist/create";

                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode && response.Content != null)
                    {
                        IsAdded = true;
                        return RedirectToPage("/Cart");
                    }
                    else
                    {
						IsAdded = false;
						return Page();
					}
                    
                }
            }
            else { return RedirectToPage("/Login"); }
		}


	}
}
