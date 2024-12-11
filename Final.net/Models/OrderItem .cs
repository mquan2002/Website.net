using Microsoft.EntityFrameworkCore;

namespace Final.net.Models
{
    public partial class OrderItem
    {
        public int OrderItemId { get; set; }  // Khóa chính của OrderItem
        public int OrderId { get; set; }      // Khóa ngoại liên kết với bảng Orders
        public virtual Order? Order { get; set; }
        public int ProductId { get; set; }    // Khóa ngoại liên kết với bảng Products
        public virtual Product? Product { get; set; }
        public int Quantity { get; set; }     // Số lượng sản phẩm
        public double UnitPrice { get; set; } // Giá sản phẩm tại thời điểm mua

        public double Price { get; set; } // Giá sản phẩm tại thời điểm mua
        public int SizeId { get; internal set; }

        public virtual Size? Size { get; set; }

        public int CrustId { get; internal set; }

        public virtual Crust? Crust { get; set; }


        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<User> Users { get; set; }
    }
}