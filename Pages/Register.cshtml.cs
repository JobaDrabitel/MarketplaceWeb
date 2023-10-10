using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Marketplace_Web.Models;
using Microsoft.EntityFrameworkCore;
using Marketplace_Web;

public class RegisterModel : PageModel
{
	public Marketplace_Web.Models.User User { get; set; } = new User();
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
	[MaxLength(11, ErrorMessage = "������������ ����� Phone ����� ���� 11 ��������")]
	[MinLength(11, ErrorMessage = "����������� ����� Phone ����� ���� 11 ��������")]
	[DataType(DataType.PhoneNumber)]
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
	[BindProperty]
	[DisplayName("Image URL")]
	[DataType(DataType.ImageUrl)]
	public string ImageUrl { get; set; } = null!;
	Marketplace1Context _context = new Marketplace1Context();

	public async Task<IActionResult> OnPost()
    {
        UserController userController = new UserController(_context);

		if (!ModelState.IsValid)
        {
            return Page();
        }

        User user = new User
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
			PasswordHash = Password,
            Roles = new List<Role> { await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "User") },
            Phone = Phone,
			ImageUrl = ImageUrl,
        };
		user = await userController.PostUser(user);
        if (user!= null)
		{
			UserSessions.SetUser(HttpContext.Session, user);
			return RedirectToPage("/Index");
		}
        else
        {
				ModelState.AddModelError(string.Empty, "������������ ������ ��� email ��� ������������");
        }
             return Page(); 
        
    }
}
