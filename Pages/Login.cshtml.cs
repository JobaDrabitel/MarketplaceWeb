using Marketplace_Web;
using Marketplace_Web.Pages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Marketplace_Web.Models;
using Microsoft.EntityFrameworkCore;

public class LoginModel : PageModel
{
	[BindProperty]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
	[MaxLength(32, ErrorMessage = "Максимальная длина email может быть 32 символа")]
	public string Email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
	[MinLength(8, ErrorMessage = "Минимальная длина пароля должна быть 8 символов")]
	[MaxLength(32, ErrorMessage = "Максимальная длина пароля может быть 32 символа")]
	[DataType(DataType.Password)]
    public string Password { get; set; }

    Marketplace1Context _context = new Marketplace1Context();
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
			return Page();
		}

		var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == Password);
        if (user != null)
        {
            UserSessions.SetUser(HttpContext.Session, user);
            if (user.Roles.First().RoleId == 1)
                return RedirectToPage("/Index");
            else if (user != null && user.Roles.First().RoleId == 2)
                return RedirectToPage("/Moderator");
            else if (user != null && user.Roles.First().RoleId == 3)
                return RedirectToPage("/Admin");
            else if (user != null && user.Roles.First().RoleId == 4)
                return RedirectToPage("/Director");
        }
        else
        {
			// Ошибка аутентификации
			ViewData["Message"] = string.Format($"Error. Wrong email or passwoed");
		}
        return Page();
    }
}
