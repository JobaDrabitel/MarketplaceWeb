using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration.UserSecrets;

public class RegisterModel : PageModel
{
    [BindProperty]
    public string FirstName { get; set; }

    [BindProperty]
    public string LastName { get; set; }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    [BindProperty]
    public string ConfirmPassword { get; set; }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var regData = new
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            PasswordHash = Password
        };

        var jsonData = JsonSerializer.Serialize(regData);

        using (var httpClient = new HttpClient())
        {
            // Замените URL на ваше API-URL
            var apiUrl = "http://localhost:8080/api/user/create";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var userId = Convert.ToInt32(await response.Content.ReadAsStringAsync());
                HttpContext.Session.SetInt32("UserId", userId); 
                HttpContext.Session.SetString("FirstName", FirstName);
                HttpContext.Session.SetString("LastName", LastName);
                HttpContext.Session.SetString("Email", Email);
                return RedirectToPage("/Index");
            }
            else
            {
                // Обработайте ошибку
                // ...
                return Page();
            }
        }
    }
}
