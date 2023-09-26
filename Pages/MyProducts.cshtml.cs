using Marketplace_Web.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace Marketplace_Web.Pages
{
    public class MyProductsModel : PageModel
    {
        public ICollection<Product> Products { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            // �������� �������� ������������ �� ������ (���������, ��� ����� ����������� ��������� �� ������)
            var userId = HttpContext.Session.GetInt32("UserId");

            // ����������� JSON-������ ��� API
            var requestData = new
            {
                SellerUserId = userId
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

                    // ��������� ������ ��������� � �������������
                    Products = products;
                }
                else
                {
                    // ����������� ������
                }
            }

            return Page();
        }

    }
}
