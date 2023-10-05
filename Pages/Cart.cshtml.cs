using Marketplace_Web.Models;
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
			using (var httpClient = new HttpClient())
            {
                var apiUrl = "http://localhost:8080/api/wishlist/getbyfields";
                var userId = HttpContext.Session.GetInt32("UserId");
				if (userId == null)
					return Page();
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
                    // ��������� ������
                    // ���������� ��������� �� ������ � ����� ������
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
			// �������� ������ �� ���� Wishlist ��� ������ ������� �����, ��� �������� ��������� ������
			List<Product> selectedProducts = await GetSelectedProducts();
			decimal? sum = 0;
			foreach (var product in selectedProducts)
			{
				if (product.StockQuantity < 1)
				{
					ModelState.AddModelError(string.Empty, $"��������� ������ ��� ���������� ������. ������ {product.Name}  ��� � �������");
					return Page();
				}
				sum += product.Price;
			}
			if (sum == 0)
			{
				return RedirectToPage("/Index");
			}
			List<OrderItem> orderItems = new List<OrderItem>();

			Order order = new Order
			{
				UserId = HttpContext.Session.GetInt32("UserId"),
				OrderDate = DateTime.Now,
				TotalAmount = sum
			};
			
			// ��������� ����� � �������� ������ �� ���� API
			var apiUrl = "http://localhost:8080/api/order/create";
			var jsonData = JsonSerializer.Serialize(order);

			using (var httpClient = new HttpClient())
			{
				var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
				var response = await httpClient.PutAsync(apiUrl, content);

				if (response.IsSuccessStatusCode)
				{
					apiUrl = "http://localhost:8080/api/order/getbyfields";
					response = await httpClient.PostAsync(apiUrl, content);
					var jsonResponse = await response.Content.ReadAsStringAsync();
					var newOrder = JsonSerializer.Deserialize<List<Order>>(jsonResponse);
					foreach (var product in selectedProducts)
					{
						// �������� ������� ������ ��� ������� ���������� ������
						OrderItem orderItem = new OrderItem
						{
							ProductId = product.ProductId,
							Quantity = 1, // ���������� ���������� ������ � ������
							Price = product.Price,
							OrderId = newOrder[0].OrderId
						};
						apiUrl = "http://localhost:8080/api/orderitem/create";
						jsonData = JsonSerializer.Serialize(orderItem);
						content = new StringContent(jsonData, Encoding.UTF8, "application/json");
						response = await httpClient.PutAsync(apiUrl, content);
						apiUrl = $"http://localhost:8080/api/product/updatebyid/{product.ProductId}";
						var data = new { StockQuantity = product.StockQuantity - 1 };
						jsonData = JsonSerializer.Serialize(data);
						content = new StringContent(jsonData, Encoding.UTF8, "application/json");
						response = await httpClient.PostAsync(apiUrl, content);
					}
						if (response.IsSuccessStatusCode)
						{
							Wishlist wishlist = new Wishlist
							{
								UserId = HttpContext.Session.GetInt32("UserId"),
							};
							jsonData = JsonSerializer.Serialize(wishlist);
							content = new StringContent(jsonData, Encoding.UTF8, "application/json");
							apiUrl = "http://localhost:8080/api/wishlist/getbyfields";
							response = await httpClient.PostAsync(apiUrl, content);
							jsonResponse = await response.Content.ReadAsStringAsync();
							var newWishlists = JsonSerializer.Deserialize<List<Wishlist>>(jsonResponse);
							foreach (var newWishlist in newWishlists) 
							{
								apiUrl = $"http://localhost:8080/api/wishlist/deletebyid/{newWishlist.WishlistId}";
								response = await httpClient.DeleteAsync(apiUrl);
								

							}
							return RedirectToPage("/Index");
						}
						else
						{
							// ����������� ������, ��������, ������� ��������� �� ������ ������������
							ModelState.AddModelError(string.Empty, "��������� ������ ��� ���������� ������.");
							return Page();
						}
					}
					return RedirectToPage("/OrderSuccess");
				}
				
			}
		}
	}

