using API_Marketplace_.net_7_v1.Models;
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
						if (String.IsNullOrEmpty(searchTerm) && categoryId != 0)
							foreach (var product in products)
							{
								if (product.Categories.First().CategoryId == categoryId)
									Products.Add(product);
							}
						else if (categoryId == 0 && !String.IsNullOrEmpty(searchTerm))
							foreach (var product in products)
							{
								if (product.Name.ToUpper().Contains(searchTerm.ToUpper()))
									Products.Add(product);
							}
						else if (!String.IsNullOrEmpty(searchTerm) && categoryId != 0)
							foreach (var product in products)
							{
								if (product.Name.ToUpper().Contains(searchTerm.ToUpper()) && product.Categories.First().CategoryId == categoryId)
									Products.Add(product);
							}
						else
							Products = products;
					Products.RemoveAll(product => product.UpdatedAt == null || product.StockQuantity == 0);
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
							if (product.Categories.First().CategoryId == Category)
								Products.Add(product);
						
						}
					Products.RemoveAll(product => product.UpdatedAt == null || product.StockQuantity == 0);
				}
				else
				{
				}
			}
			return Page();
		}

	}
}
