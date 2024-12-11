using System.ComponentModel.DataAnnotations;

namespace Final.net.Models
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng không bỏ trống")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng không bỏ trống")]
        public string VoucherCode { get; set; }

        [Required(ErrorMessage = "Vui lòng không bỏ trống")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá tiền không phù hợp")]
        public double DiscountPrice { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Giá tiền không phù hợp")]
        public double MinPrice { get; set; }

        public bool IsActive { get; set; }
    }
}
