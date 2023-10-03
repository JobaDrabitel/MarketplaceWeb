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
	[Required(ErrorMessage = "���� Email �����������")]
	[EmailAddress(ErrorMessage = "Email ����� ������������ ������")]
	[MaxLength(32, ErrorMessage = "������������ ����� email ����� ���� 32 �������")]
	public string Email { get; set; }
	[BindProperty]
	[DisplayName("Phone")]
	[Required(ErrorMessage = "���� Phone �����������")]
	[Phone(ErrorMessage = "Phone ����� ������������ ������")]
	[StringLength(11, ErrorMessage = "����� ������ �������� �� 11 ��������")]
	public string Phone { get; set; }
	[BindProperty]
	[DisplayName("������")]
	[Required(ErrorMessage = "���� ������ �����������")]
	[MinLength(8, ErrorMessage = "����������� ����� ������ ������ ���� 8 ��������")]
	[MaxLength(32, ErrorMessage = "������������ ����� ������ ����� ���� 32 �������")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = null!;
	[BindProperty]
	[DisplayName("������������� ������")]
	[Required(ErrorMessage = "���� ������������� ������ �����������")]
	[Compare(nameof(Password), ErrorMessage = "������������� ������ ������ ��������� � �������")]
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
            // �������� URL �� ���� API-URL
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
				ModelState.AddModelError(string.Empty, "���� email ��� ������������.");
            }
             return Page(); 
        }
    }
}
