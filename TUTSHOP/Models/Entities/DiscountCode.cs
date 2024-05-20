using System.ComponentModel.DataAnnotations;

namespace TUTSHOP.Models.Entities
{
    public class DiscountCode
    {
        public int DiscountCodeId { get; set; }

        [Required]
        public string Code { get; set; }

        // Số tiền giảm giá (VD: 20% giảm giá)
        [Required]
        [Display(Name ="Số tiền giảm giá")]
        public decimal DiscountAmount { get; set; }

        // Loại giảm giá (phần trăm, số tiền cố định, miễn phí vận chuyển, ...)
        [Required]
        [Display(Name = "Loại")]
        public DiscountType Type { get; set; }

        // Điều kiện áp dụng giảm giá (VD: Tổng giá trị đơn hàng >= $100)
        [Display(Name = "Điều kiện")]
        public string? Condition { get; set; }

        // Ngày bắt đầu áp dụng giảm giá
  
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime StartDate { get; set; }

        // Ngày kết thúc áp dụng giảm giá
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime EndDate { get; set; }

        [Display(Name = "Số lượng")]
        [Required]
        public int Quantity { get; set; }

        // Các thuộc tính khác nếu cần thiết
    }

    public enum DiscountType
    {
        [Display(Name = "Phần trăm")]
        Percentage,

        [Display(Name = "Số tiền cố định")]
        FixedAmount,

        [Display(Name = "Miễn phí giao hàng")]
        FreeShipping,

        // Thêm các giá trị khác nếu cần
    }
}

