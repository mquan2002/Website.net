using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Final.net.Models;

public partial class Product
{
    public int ProductId { get; set; }

    [MaxLength(255)]
    public string ProductName { get; set; } = null!;

    public double Price { get; set; }

    public string Description { get; set; } = null!;

    [MaxLength(255)]
    public string? ImageUrl { get; set; } = null!;

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<CartItem> CartItems { get; set; }

}

