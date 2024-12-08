using System;
using System.Collections.Generic;

namespace Final.net.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string Method { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

}