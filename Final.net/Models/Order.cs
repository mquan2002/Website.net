namespace Final.net.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }

        public String Address { get; set; }

   
        public string SDT { get; set; }
        public double? TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public int PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }

        public int DeliveryId { get; set; }
        public virtual Delivery? Delivery { get; set; }

        public string PaymentStatus { get; set; } = null!;
        public string PaymentMethod { get; internal set; }

        public string Notes { get; internal set; }

        public int UserId { get; set; }

        public virtual User? User { get; set; }

        // Quan hệ với bảng OrderItems (mỗi đơn hàng có thể có nhiều sản phẩm)
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}