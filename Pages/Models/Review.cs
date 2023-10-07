using System;
using System.Collections.Generic;

namespace API_Marketplace_.net_7_v1.Models;

public partial class Review
{
    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }
	public string? ImageUrl { get; set; }

	public DateTime? CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
