using Marketplace_Web.Models;

namespace Marketplace_Web
{
    public  static partial class UserSessions
	{
		public static void SetUser(this ISession session, User user)
		{
			session.SetInt32("UserId", user.UserId);
			session.SetString("FirstName", user.FirstName);
			session.SetString("LastName", user.LastName);
			session.SetString("Email", user.Email);
			session.SetString("ImageUrl", user.ImageUrl);
		}

		public static User GetUser(this ISession session)
		{
			var userId = session.GetInt32("UserId");
			var firstName = session.GetString("FirstName");
			var lastName = session.GetString("LastName");
			var email = session.GetString("Email");
			var ImageUrl = session.GetString("ImageUrl");

			return new User
			{
				UserId = userId ?? 0,
				FirstName = firstName,
				LastName = lastName,
				Email = email,
				ImageUrl = ImageUrl
			};
		}
	}
}
