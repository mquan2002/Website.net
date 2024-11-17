using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.net.Models
{
    public class Role : BaseEntity

    {
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}