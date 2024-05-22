using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
    public interface IOrderRepository
    {
       
        IEnumerable<Order> GetAll();
        IEnumerable<Order> GetOrdersByUserId(string userId);
        IEnumerable<Order> GetPendingOrders(); // Lấy đơn hàng chưa duyệt
        IEnumerable<Order> GetApprovedOrders(); // Lấy đơn hàng đã duyệt
        Order GetById(int id);
        void ApproveOrder(int id); // Duyệt đơn hàng
        void CancelOrder(int id);
        void DeleteOrder(int id);
        void UpdateOrderStatus(int orderId, OrderStatus status);
    }
}
