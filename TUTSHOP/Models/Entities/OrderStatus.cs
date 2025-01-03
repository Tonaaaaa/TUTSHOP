using System.ComponentModel.DataAnnotations;

namespace TUTSHOP.Models.Entities
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Unpaid,    // Chưa thanh toán
        Paid       // Đã thanh toán
    }
}
