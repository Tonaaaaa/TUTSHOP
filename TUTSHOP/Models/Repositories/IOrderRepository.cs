using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
	public interface IOrderRepository
	{
		IEnumerable<Order> GetAll();
        IEnumerable<Order> GetOrdersByUserId(string userId);
        Order GetById(int id);
		void CancelOrder(int id);
		void DeleteOrder(int id);
	}
}
