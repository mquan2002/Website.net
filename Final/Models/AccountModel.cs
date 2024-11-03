using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Account : BaseModel
    {

        [Required(ErrorMessage = "Username không được để trống")]
        [StringLength(50, ErrorMessage = "Username không dài hơn 50 ký tự")]
        [MinLength(5, ErrorMessage = "Username phải có ít nhất 5 ký tự")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Username chỉ có thể chứa chữ cái và số")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        // [StringLength(255, ErrorMessage = "Mật khẩu không dài hơn 255 ký tự")]
        [MinLength(5, ErrorMessage = "Mật khẩu phải có ít nhất 5 ký tự")]
        public string Password { get; set; }


        public bool IsLoggedIn { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime? LastLogin { get; set; } = null;


        public int RoleId { get; set; }

        public Role Role { get; set; }





    }
}