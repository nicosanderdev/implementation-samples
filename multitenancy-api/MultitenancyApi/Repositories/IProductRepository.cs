using MultitenancyApi.Models;

namespace MultitenancyApi.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> AddAsync(Product product);
        void Update(Product product);
        void Delete(Product product);
        Task<int> SaveChangesAsync();
    }
}