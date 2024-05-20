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
			return _context.Orders;
		}

		public Order GetById(int id)
		{
			return _context.Orders.Find(id);
		}

		public void CancelOrder(int id)
		{
			var order = _context.Orders.Find(id);
			if (order != null)
			{
				// Thực hiện logic hủy đơn hàng ở đây
				// Ví dụ: đặt trạng thái đơn hàng thành "Đã hủy"
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

        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            return _context.Orders.Where(o => o.UserId == userId).ToList();
        }
    }
}
