using Marketplace_Web;
using Marketplace_Web.Models;
using Marketplace_Web.Pages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class LoginModel : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Создайте объект для отправки на сервер
        var loginData = new
        {
            Email = Email,
            PasswordHash = Password
        };

        // Сериализуйте объект в JSON
        var jsonData = JsonSerializer.Serialize(loginData);

        // Отправьте POST-запрос на API
        using (var httpClient = new HttpClient())
        {
            // Замените URL на ваше API-URL
            var apiUrl = "http://localhost:8080/api/user/getbyfields";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode && response.Content!=null)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                jsonResponse = JsonFormatter.JSONFormatting(jsonResponse);
                User? user;

				try
                {
                    user = JsonSerializer.Deserialize<User>(jsonResponse);
                }
                catch { user = null; }
				if (user != null)
				{
					HttpContext.Session.SetInt32("UserId", user.UserId);
					HttpContext.Session.SetString("FirstName", user.FirstName);
					HttpContext.Session.SetString("LastName", user.LastName);
					HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetInt32("RoleId", (int)user.RoleId);
					if (user.ImageUrl != null)
						HttpContext.Session.SetString("ImageUrl", user.ImageUrl);
                    IndexModel.user = user;
				}
                
                if (user != null && user.RoleId == 1)
                    return RedirectToPage("/Index");
                else if (user != null && user.RoleId == 2)
                    return RedirectToPage("/Admin");
                else if (user != null && user.RoleId == 3)
                    return RedirectToPage("/Moderator");
				else if (user != null && user.RoleId == 4)
					return RedirectToPage("/Director");
				else return Page();
			}
            else
            {
                // Ошибка аутентификации
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }
        }
    }
}
