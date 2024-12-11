using System;
using System.Collections.Generic;

namespace Final.net.Models;

public partial class Crust
{
    public int CrustId { get; set; }

    public string CrustName { get; set; } = null!;

    public ICollection<CartItem> CartItems { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }

}
