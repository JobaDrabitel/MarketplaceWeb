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
				CreateResult = "Пользователь успешно создан!";
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Этот email уже используется или поля некорректны.");
				CreateResult = "Ошибка при добавлении пользователя";
			}
			}
			catch (Exception ex) { return null; }
		}
			return Page();
	}
}
