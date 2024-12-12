using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Final.net.Models
{
    public partial class Category
    {
        public int CategoryId { get; set; }

        [MaxLength(255)]

        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Tên thể loại không được chứa số hoặc ký tự đặc biệt")]
        [Required(ErrorMessage = "Tên thể loại không được rỗng")]
        public string CategoryName { get; set; } = null!;

        [MaxLength(255)]
        public string? CategoryImage { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
