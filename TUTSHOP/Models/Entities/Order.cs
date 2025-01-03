using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TUTSHOP.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        [Required(ErrorMessage ="Hãy nhập địa chỉ")]
        public string? ShippingAddress { get; set; }
        public string? Notes { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public bool IsApproved { get; set; } // Thêm thuộc tính này
    }

}
