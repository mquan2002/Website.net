using System;
using Final.net.Controllers;
using Final.net.Models;

namespace Final.net.Models
{
    public class CartItem 
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public double BasePrice { get; set; } // Giá gốc của sản phẩm

        // Giá một đơn vị sản phẩm
        public double Price => BasePrice + (Size?.SizeCost ?? 0);

        // Tổng giá (bao gồm số lượng)
        public double TotalPrice => Price * Quantity;
        public int Quantity { get; set; } = 1; // Thiết lập số lượng mặc định là 1
        public required string ImageUrl { get; set; }

        public int SizeId { get; set; }
        public Size Size { get; set; } // Không nullable nếu tất cả sản phẩm cần Size

        public int CrustId { get; set; }
        public Crust Crust { get; set; } // Không nullable nếu tất cả sản phẩm cần Crust

        // Phương thức tính tổng giá cho một mục giỏ hàng, bao gồm giá cơ bản và các chi phí thêm từ Size
        


        // Định dạng giá thành VND
        public string TotalPriceFormatted => TotalPrice.ToString("N0") + "₫";
    }
}
