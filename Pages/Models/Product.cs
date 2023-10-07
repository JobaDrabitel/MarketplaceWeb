using System;
using System.Collections.Generic;

namespace API_Marketplace_.net_7_v1.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? StockQuantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
	public string? ImageUrl { get; set; }


	public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<User> UsersNavigation { get; set; } = new List<User>();
}
