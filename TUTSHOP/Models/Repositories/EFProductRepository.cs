using Microsoft.EntityFrameworkCore;
using TUTSHOP.Data_Access;
using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;


        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.Products.Include(p => p.Category).ToListAsync();


        }


        public async Task<Product> GetByIdAsync(int id)
        {
            // return await _context.Products.FindAsync(id);
            // lấy thông tin kèm theo category
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);
        }


        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Product>> SearchAsync(string keyword)
        {
            return await _context.Products
                                 .Where(p => p.ProductName.Contains(keyword) || p.Description.Contains(keyword))
                                 .Include(p => p.Category)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Where(p => p.Category.CategoryName == category)
                                 .ToListAsync();
        }
    }
}
