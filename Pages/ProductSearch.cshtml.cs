using Marketplace_Web.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace Marketplace_Web.Pages
{
    public class ProductSearchModel : PageModel
    {
        public List<Product>? Products { get; set; } = new List <Product>();
        public async Task<IActionResult> OnPostAsync(string searchTerm)
        {

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
                        if (product.Name.ToUpper().Contains(searchTerm.ToUpper()))
                            Products.Add(product);
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
