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
        public ProfileModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public User User { get; set; } // Здесь User - это модель пользователя, которую вы используете

        public bool IsUpdated { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var apiUrl = $"http://localhost:8080/api/user/getbyid/{userId}";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    User = JsonSerializer.Deserialize<User>(jsonResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotAcceptable)
                {
                    ModelState.AddModelError(string.Empty, "Этот email уже используется.");
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var updateData = new
            {
                FirstName,
                LastName,
                Email,
                PasswordHash = Password,
                Phone,
                ImageUrl
            };

            var jsonData = JsonSerializer.Serialize(updateData);

            using (var httpClient = new HttpClient())
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var apiUrl = $"http://localhost:8080/api/user/updatebyid/{userId}";

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    User = new User
                    {
                        FirstName = updateData.FirstName,
                        LastName = updateData.LastName,
                        Email = updateData.Email,
                        Phone = updateData.Phone,
                        PasswordHash = updateData.PasswordHash,
                        ImageUrl = updateData.ImageUrl
                    };
                    IsUpdated = true;
                }
                
                else if (response.StatusCode == System.Net.HttpStatusCode.NotAcceptable)
                {
                    ModelState.AddModelError(string.Empty, "Этот email уже используется.");
                }
                return Page();
            }
        }
    }
}
