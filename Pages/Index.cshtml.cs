using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using System.Net;
using Marketplace_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public  User user;
		public int? currentRole;
		private readonly IMemoryCache _cache;
		public static  IEnumerable<Category> Categories { get; private set; }
		public static  IEnumerable<Product> Products { get; private set; }
		public int Rating { get; private set; }
		Marketplace1Context _context = new Marketplace1Context();
		public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
			Categories = await GetCategories();

			currentRole = HttpContext.Session.GetInt32("RoleId");
			user =  UserSessions.GetUser(HttpContext.Session);
			if (user != null && currentRole == 2)
				return RedirectToPage("/Moderator");
			else if (user != null && currentRole == 3)
				return RedirectToPage("/Admin");
			else if (user != null && currentRole == 4)
				return RedirectToPage("/Director");
			Products = await GetProducts();
			return Page();
		}
        public  async Task<IEnumerable<Category>> GetCategories()
        {
			return await _context.Categories.ToListAsync();
		}
		public async Task<IEnumerable<Product>> GetProducts()
		{
			var products = await _context.Products.ToListAsync();
			products.RemoveAll(product => product.UpdatedAt == null || product.StockQuantity == 0);
			return products;

		}
		public async Task<IActionResult> OnPostAddToCartAsync(int productId)
		{
			var cart = _cache.Get<Dictionary<int, int>>("Cart") ?? new Dictionary<int, int>();
			if (cart.ContainsKey(productId))
			{
				cart[productId]++;
			}
			else
			{
				cart[productId] = 1;
			}
			_cache.Set("Cart", cart);
			return RedirectToPage("/Index"); 
		}
		public IActionResult OnGetLogout(string returnUrl)
		{
			HttpContext.Session.Clear();
			user = null;
			return RedirectToPage("/Index"); 
		}
		[HttpPost]
		public IActionResult OnPostSubscribe(string email)
		{
			 SendMessageAsync(email);

			return Page();
		}
		static async Task SendMessageAsync(string email)
		{
			string fromEmail = "legenadary.pigeon@gmail.com"; 
			string fromEmailPassword = "burz uiew yaqc zfrq"; 
			string toEmail = email; 

			SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential(fromEmail, fromEmailPassword),
				EnableSsl = true,
			};

			MailMessage mailMessage = new MailMessage
			{
				From = new MailAddress(fromEmail),
				Subject = "Subscribe",
				Body = "I am subscribe and you have been subscribed!",
			};

			mailMessage.To.Add(toEmail);

			try
			{
				smtpClient.Send(mailMessage);
				Console.WriteLine("Письмо успешно отправлено.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при отправке письма: {ex.Message}");
			}
		}
		
	}
}