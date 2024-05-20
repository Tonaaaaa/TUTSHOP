using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
	public interface IContactRepository
	{
		Task<IEnumerable<Contact>> GetAllAsync();
		void Add(Contact message);
		void SaveMessage(Contact message);
	}
}
