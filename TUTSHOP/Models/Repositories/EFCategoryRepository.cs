using Microsoft.EntityFrameworkCore;
using TUTSHOP.Data_Access;
using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.Categories.Include(x => x.Products).ToListAsync();
            // Include thông tin về product

        }


        public async Task<Category> GetByIdAsync(int id)
        {
            // return await _context.Products.FindAsync(id);
            // lấy thông tin kèm theo category
            return await _context.Categories.Include(p => p.Products).FirstOrDefaultAsync(p => p.CategoryId == id);
        }


        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> SearchAsync(string keyword)
        {
            return await _context.Categories
                .Where(c => c.CategoryName.Contains(keyword))
                .ToListAsync();
        }
    }
}
