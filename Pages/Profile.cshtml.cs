using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Marketplace_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace Marketplace_Web
{
	public class ProfileModel : PageModel
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
		[MaxLength(15, ErrorMessage = "Максимальная длина Phone может быть 15 символов")]
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
		[DisplayName("Фото профиля")]
		[DataType(DataType.Url)]
		[MaxLength(256, ErrorMessage = "Максимальная длина url может быть 256 символов")]
		public string ImageUrl { get; set; } = null!;

		private readonly IHttpClientFactory _clientFactory;
		Marketplace1Context _context = new Marketplace1Context();
		public ProfileModel(IHttpClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}

		public User User { get; set; } // Здесь User - это модель пользователя, которую вы используете

		public bool IsUpdated { get; private set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var userId = HttpContext.Session.GetInt32("UserId");
			User = await _context.Users.FindAsync(userId);
			return Page();

		}

		public async Task<IActionResult> OnPostAsync()
		{
			var userId = HttpContext.Session.GetInt32("UserId");
			UserController userController = new UserController(_context);
			if (!ModelState.IsValid)
			{
				await OnGetAsync();
				return Page();
			}

			User newUser = new User
			{
				UserId = (int)userId,
				FirstName = FirstName,
				LastName = LastName,
				Email = Email,
				PasswordHash = Password,
				Phone = Phone,
				ImageUrl = ImageUrl,
			};

			try
			{
				User = await userController.PutUser((int)userId, newUser);
				
			}

			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
			}
			return await OnGetAsync();

		}
	}
}
