using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Final.net.Models;

public partial class User : BaseEntity
{
    [Required(ErrorMessage = "Tên người dùng không được để trống.")]
    [StringLength(50, ErrorMessage = "Tên người dùng không được vượt quá 50 ký tự.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
    public string Password { get; set; } = null!;


    public string? Email { get; set; }

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int RoleId { get; set; }

    public Role Role { get; set; }
}
