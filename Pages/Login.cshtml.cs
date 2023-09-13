using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

        // �������� ������ ��� �������� �� ������
        var loginData = new
        {
            Email = Email,
            PasswordHash = Password
        };

        // ������������ ������ � JSON
        var jsonData = JsonSerializer.Serialize(loginData);

        // ��������� POST-������ �� API
        using (var httpClient = new HttpClient())
        {
            // �������� URL �� ���� API-URL
            var apiUrl = "http://localhost:8080/api/user/auth";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                // ����������� �������
                // ����� ����� ��������� �������������� ��������, ��������, ������������� �� ������ ��������
                return RedirectToPage("/Index");
            }
            else
            {
                // ������ ��������������
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }
        }
    }
}
