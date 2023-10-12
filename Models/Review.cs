﻿using System;
using System.Collections.Generic;

namespace Marketplace_Web.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
