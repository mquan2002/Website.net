using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Role : BaseModel
    {
        [Required(ErrorMessage = "Name không được để trống.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "CodeName không được để trống.")]
        public string CodeName { get; set; }

        public ICollection<Account> Accounts { get; set; }

    }
}