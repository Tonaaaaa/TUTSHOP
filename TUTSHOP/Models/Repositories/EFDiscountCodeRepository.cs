using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUTSHOP.Data_Access;
using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
    public class EFDiscountCodeRepository : IDiscountCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public EFDiscountCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DiscountCode>> GetAllAsync()
        {
            return await _context.DiscountCodes.ToListAsync();
        }

        public async Task<DiscountCode> GetByIdAsync(int id)
        {
            return await _context.DiscountCodes.FindAsync(id);
        }

        public async Task AddAsync(DiscountCode discountCode)
        {
            _context.DiscountCodes.Add(discountCode);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DiscountCode discountCode)
        {
            _context.DiscountCodes.Update(discountCode);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode != null)
            {
                _context.DiscountCodes.Remove(discountCode);
                await _context.SaveChangesAsync();
            }
        }
    }
}
