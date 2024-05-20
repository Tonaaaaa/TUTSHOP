using TUTSHOP.Models.Entities;

namespace TUTSHOP.Models.Repositories
{
    public interface IDiscountCodeRepository
    {
        Task<List<DiscountCode>> GetAllAsync();
        Task<DiscountCode> GetByIdAsync(int id);
        Task AddAsync(DiscountCode discountCode);
        Task UpdateAsync (DiscountCode discountCode);
        Task DeleteAsync (int id);
    }
}
