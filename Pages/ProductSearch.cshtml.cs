using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.Diagnostics.Contracts;

namespace Marketplace_Web.Pages
{
    public class ProductSearchModel : PageModel
    {
        public int CategoryId { get; set; }
        public List<Product>? Products { get; set; } = new List <Product>();
        public async Task<IActionResult> OnPostAsync(string searchTerm, int categoryId)
        {
            CategoryId = categoryId;
            using (var httpClient = new HttpClient())
            {
                var apiUrl = "http://localhost:8080/api/product/getall";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var productsJson = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<Product>>(productsJson);
                    if (products != null)
                    if (String.IsNullOrEmpty(searchTerm))
						foreach (var product in products)
						{
							if (product.CategoryId == categoryId)
								Products.Add(product);
						}
                    else if (categoryId == 0)
						foreach (var product in products)
						{
							if (product.Name.ToUpper().Contains(searchTerm.ToUpper()))
								Products.Add(product);
						}
                    else
					    foreach (var product in products)
                        {
						    if (product.Name.ToUpper().Contains(searchTerm.ToUpper()) && product.CategoryId == categoryId)
                                Products.Add(product);
                        }
                }
                else
                {
                }
            }

            return Page();
        }
		public async Task<IActionResult> OnGet(int Category)
		{
			CategoryId = Category;
			using (var httpClient = new HttpClient())
			{
				var apiUrl = "http://localhost:8080/api/product/getall";
				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					var productsJson = await response.Content.ReadAsStringAsync();
					var products = JsonSerializer.Deserialize<List<Product>>(productsJson);
					if (products != null)
						
							foreach (var product in products)
							{
								if (product.CategoryId == Category)
									Products.Add(product);
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
