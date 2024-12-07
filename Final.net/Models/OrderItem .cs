using Final.net.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }  // Khóa chính của OrderItem
    public int OrderId { get; set; }      // Khóa ngoại liên kết với bảng Orders
    public int ProductId { get; set; }    // Khóa ngoại liên kết với bảng Products
    public int Quantity { get; set; }     // Số lượng sản phẩm
    public double UnitPrice { get; set; } // Giá sản phẩm tại thời điểm mua

    // Quan hệ với bảng Order
    public Order Order { get; set; }

    // Quan hệ với bảng Product
    public Product Product { get; set; }
    public double Price { get; internal set; }
    public int SizeId { get; internal set; }
    public int CrustId { get; internal set; }
}
