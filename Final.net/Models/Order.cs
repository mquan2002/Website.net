using System;
using System.Collections.Generic;

namespace Final.net.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public double? TotalAmount { get; set; }

    public DateTime OrderDate { get; set; }

    public string PaymentStatus { get; set; } = null!;
}
