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
        public ProfileModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public User User { get; set; } // ����� User - ��� ������ ������������, ������� �� �����������

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
                    ModelState.AddModelError(string.Empty, "���� email ��� ������������.");
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
                    ModelState.AddModelError(string.Empty, "���� email ��� ������������.");
                }
                return Page();
            }
        }
    }
}
