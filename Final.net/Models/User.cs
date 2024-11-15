using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace Final.net.Models;

public class User 
{
    [Key]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Tên người dùng không được để trống.")]
    [StringLength(50, ErrorMessage = "Tên người dùng không được vượt quá 50 ký tự.")]
    public string UserName { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Địa chỉ không được để trống.")]
    [StringLength(100, ErrorMessage = "Địa chỉ không được vượt quá 100 ký tự.")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
    public string Phone { get; set; } = null!;

    public DateTime CreateOn { get; set; } = DateTime.Now;




}
