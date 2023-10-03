using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

public class RegisterModel : PageModel
{
    [BindProperty]
    public string FirstName { get; set; }

    [BindProperty]
    public string LastName { get; set; }

	[BindProperty]
	[DisplayName("Email")]
	[Required(ErrorMessage = "Поле Email обязательно")]
	[EmailAddress(ErrorMessage = "Email имеет неправильный формат")]
	[MaxLength(32, ErrorMessage = "Максимальная длина email может быть 32 символа")]
	public string Email { get; set; }
	[BindProperty]
	[DisplayName("Phone")]
	[Required(ErrorMessage = "Поле Phone обязательно")]
	[Phone(ErrorMessage = "Phone имеет неправильный формат")]
	[StringLength(11, ErrorMessage = "Номер должен состоять из 11 символов")]
	public string Phone { get; set; }
	[BindProperty]
	[DisplayName("Пароль")]
	[Required(ErrorMessage = "Поле Пароль обязательно")]
	[MinLength(8, ErrorMessage = "Минимальная длина пароля должна быть 8 символов")]
	[MaxLength(32, ErrorMessage = "Максимальная длина пароля может быть 32 символа")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = null!;
	[BindProperty]
	[DisplayName("Подтверждение пароля")]
	[Required(ErrorMessage = "Поле Подтверждение пароля обязательно")]
	[Compare(nameof(Password), ErrorMessage = "Подтверждение пароля должно совпадать с паролем")]
	[DataType(DataType.Password)]
	public string ConfirmPassword { get; set; } = null!;

	public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var regData = new
        {
            FirstName,
            LastName,
            Email,
            PasswordHash = Password,
            RoleId = 2,
            Phone,
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
            else if (response.StatusCode == System.Net.HttpStatusCode.NotAcceptable)
            {
				ModelState.AddModelError(string.Empty, "Этот email уже используется.");
            }
             return Page(); 
        }
    }
}
