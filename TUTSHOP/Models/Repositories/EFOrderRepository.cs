using Microsoft.EntityFrameworkCore;
using TUTSHOP.Data_Access;
using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public EFOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetAll()
        {
            return _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToList();
        }

        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            return _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.ApplicationUser)  // Bao gồm dữ liệu ApplicationUser
                .ToList();
        }

        public IEnumerable<Order> GetPendingOrders()
        {
            return _context.Orders.Where(o => !o.IsApproved).ToList();
        }

        public IEnumerable<Order> GetApprovedOrders()
        {
            return _context.Orders.Where(o => o.IsApproved).ToList();
        }

        public Order GetById(int id)
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.ApplicationUser)
                .FirstOrDefault(o => o.Id == id);
        }

        public void ApproveOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.IsApproved = true;
                _context.SaveChanges();
            }
        }

        public void CancelOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.OrderStatus = OrderStatus.Cancelled;
                _context.SaveChanges();
            }
        }

        public void DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                _context.SaveChanges();
            }
        }
    }
}