using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net.Http;
using System.Globalization;
using Marketplace_Web.Models;

public class DirectorCreateUser : PageModel
{
	public string CreateResult { get; set; }
	public List<Role> Roles = new List<Role>();
	[BindProperty]
	public string FirstName { get; set; }

	[BindProperty]
	public string LastName { get; set; }
	[BindProperty]
	public string ImageUrl { get; set; }
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
	Marketplace1Context _context = new Marketplace1Context();
	public async Task<IActionResult> OnGet(Role selectedRole)
	{
		if (HttpContext.Session.GetInt32("RoleId") < 4)
			return RedirectToPage("/Index");
		Roles =  _context.Roles.ToList();
		return Page();
	}
	public async Task<IActionResult> OnPost(string selectedRole)
	{
		Roles = _context.Roles.ToList();
		if (!ModelState.IsValid)
		{
			return Page();
		}
		var role = Roles.FirstOrDefault(r => r.RoleName == selectedRole);
		User user = new User()
		{
			FirstName = FirstName,
			LastName = LastName,
			Email = Email,
			PasswordHash = Password,
			Roles = new List<Role>() { role},
			ImageUrl = ImageUrl,
			Phone = Phone,
		};
		if (user != null)
		{
			try
			{
				var newUser = await _context.Users.AddAsync(user);
				await _context.SaveChangesAsync();
			

			if (newUser != null)
			{
				CreateResult = "������������ ������� ������!";
			}
			else
			{
				ModelState.AddModelError(string.Empty, "���� email ��� ������������ ��� ���� �����������.");
				CreateResult = "������ ��� ���������� ������������";
			}
			}
			catch (Exception ex) { return null; }
		}
			return Page();
	}
}
