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
		[Required(ErrorMessage = "���� Email �����������")]
		[EmailAddress(ErrorMessage = "Email ����� ������������ ������")]
		[MaxLength(32, ErrorMessage = "������������ ����� email ����� ���� 32 �������")]
		public string Email { get; set; }
		[BindProperty]
		[DisplayName("Phone")]
		[Required(ErrorMessage = "���� Phone �����������")]
		[Phone(ErrorMessage = "Phone ����� ������������ ������")]
		[MaxLength(15, ErrorMessage = "������������ ����� Phone ����� ���� 15 ��������")]
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
		[DisplayName("���� �������")]
		[DataType(DataType.Url)]
		[MaxLength(256, ErrorMessage = "������������ ����� url ����� ���� 256 ��������")]
		public string ImageUrl { get; set; } = null!;

		private readonly IHttpClientFactory _clientFactory;
		Marketplace1Context _context = new Marketplace1Context();
		public ProfileModel(IHttpClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}

		public User User { get; set; } // ����� User - ��� ������ ������������, ������� �� �����������

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
