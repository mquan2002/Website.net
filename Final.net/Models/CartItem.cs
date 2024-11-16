using System;
using Final.net.Models;

namespace Final.net.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; } = 1; // Thiết lập số lượng mặc định là 1
        public required string ImageUrl { get; set; }

        public int SizeId { get; set; }
        public Size Size { get; set; } // Không nullable nếu tất cả sản phẩm cần Size

        public int CrustId { get; set; }
        public Crust Crust { get; set; } // Không nullable nếu tất cả sản phẩm cần Crust

        // Phương thức tính tổng giá cho một mục giỏ hàng, bao gồm giá cơ bản và các chi phí thêm từ Size
        public double TotalPrice => Price * Quantity;


        // Định dạng giá thành VND
        public string TotalPriceFormatted => TotalPrice.ToString("N0") + "₫";
    }
}
