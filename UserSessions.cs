using Marketplace_Web.Models;
using System.Data;

namespace Marketplace_Web
{
    public  static partial class UserSessions
	{
		public static void SetUser(this ISession session, User user)
		{
			try
			{
				session.SetInt32("UserId", user.UserId);
				session.SetString("FirstName", user.FirstName);
				session.SetString("LastName", user.LastName);
				session.SetString("Email", user.Email);
				session.SetString("ImageUrl", user.ImageUrl);
				session.SetString("Phone", user.Phone);
				session.SetInt32("RoleId", (int)user.Roles.First().RoleId);
			}
			catch (Exception ex) { }
		}

		public static User GetUser(this ISession session)
		{
			var firstName = session.GetString("FirstName");
			var lastName = session.GetString("LastName");
			var email = session.GetString("Email");
			var ImageUrl = session.GetString("ImageUrl");
			var phone = session.GetString("Phone");
			return new User
			{
				FirstName = firstName,
				LastName = lastName,
				Email = email,
				ImageUrl = ImageUrl,
				Phone = phone,
			};
		}
	}
}
