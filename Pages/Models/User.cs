using System;
using System.Collections.Generic;

namespace API_Marketplace_.net_7_v1.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }
	public string? Phone { get; set; }
	public string? ImageUrl { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Product> ProductsNavigation { get; set; } = new List<Product>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
