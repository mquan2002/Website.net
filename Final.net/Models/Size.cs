using System;
using System.Collections.Generic;

namespace Final.net.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public string SizeName { get; set; } = null!;

    public double? SizeCost { get; set; }
}
