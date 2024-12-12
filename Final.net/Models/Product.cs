using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Final.net.Models;

public partial class Product
{
    public int ProductId { get; set; }

    [MaxLength(255)]
    [Required(ErrorMessage = "Tên sản phẩm không được rỗng")]
    public string ProductName { get; set; } = null!;

    [Required(ErrorMessage = "Giá sản phẩm không được rỗng")]
    public double Price { get; set; }

    [Required(ErrorMessage = "Mô tả không được rỗng")]
    public string Description { get; set; } = null!;

    [MaxLength(255)]
    public string? ImageUrl { get; set; } = null!;

    [Required(ErrorMessage = "Thể loại không được rỗng")]
    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

}

