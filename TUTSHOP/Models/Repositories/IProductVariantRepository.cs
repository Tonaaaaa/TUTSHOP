using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
    public interface IProductVariantRepository
    {
        Task<IEnumerable<ProductVariant>> GetAllAsync();
        Task<ProductVariant> GetByIdAsync(int id);
        Task AddAsync(ProductVariant productVariant);
        Task UpdateAsync(ProductVariant productVariant);
        Task DeleteAsync(int id);
    }
}
