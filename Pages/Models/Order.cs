using System;
using System.Collections.Generic;

namespace API_Marketplace_.net_7_v1.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? TotalQuantity { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
