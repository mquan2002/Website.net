using System;
using System.Collections.Generic;

namespace Final.net.Models;

public partial class Delivery
{
    public int DeliveryId { get; set; }

    public DateTime DeliveryDate { get; set; }

    public string DeliveryStatus { get; set; } = null!;
}
