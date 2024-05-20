using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TUTSHOP.Data_Access;
using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
    public class EFProductVariantRepository : IProductVariantRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductVariantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync()
        {
            return await _context.ProductVariants
                .Include(p => p.Product)
                .ToListAsync();
        }

        public async Task<ProductVariant> GetByIdAsync(int id)
        {
            return await _context.ProductVariants
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.ProductVariantId == id);
        }

        public async Task AddAsync(ProductVariant productVariant)
        {
            _context.ProductVariants.Add(productVariant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductVariant productVariant)
        {
            _context.Entry(productVariant).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var productVariant = await GetByIdAsync(id);
            if (productVariant != null)
            {
                _context.ProductVariants.Remove(productVariant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
