using Microsoft.EntityFrameworkCore;
using TUTSHOP.Data_Access;
using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
	public class EFContactRepository : IContactRepository
	{
		private readonly ApplicationDbContext _context;

		public EFContactRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public void Add(Contact message)
		{
			_context.Contacts.Add(message);
			_context.SaveChanges();
		}

		public async Task<IEnumerable<Contact>> GetAllAsync()
		{
			return await _context.Contacts.ToListAsync();
		}

		public void SaveMessage(Contact message)
		{
			_context.Contacts.Add(message); // Thêm đối tượng message vào DbContext
			_context.SaveChanges();
		}
	}
}
