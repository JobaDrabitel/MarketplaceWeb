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
	[Required(ErrorMessage = "Поле Email обязательно")]
	[EmailAddress(ErrorMessage = "Email имеет неправильный формат")]
	[MaxLength(32, ErrorMessage = "Максимальная длина email может быть 32 символа")]
	public string Email { get; set; }
	[BindProperty]
	[DisplayName("Phone")]
	[Required(ErrorMessage = "Поле Phone обязательно")]
	[Phone(ErrorMessage = "Phone имеет неправильный формат")]
	[MaxLength(11, ErrorMessage = "Максимальная длина Phone может быть 11 символов")]
	[MinLength(11, ErrorMessage = "Минимальная длина Phone может быть 11 символов")]
	[DataType(DataType.PhoneNumber)]
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
				ModelState.AddModelError(string.Empty, "Некорректные данные или email уже используется");
        }
             return Page(); 
        
    }
}
