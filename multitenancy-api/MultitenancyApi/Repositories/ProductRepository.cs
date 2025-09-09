using Microsoft.EntityFrameworkCore;
using MultitenancyApi.Data;
using MultitenancyApi.Models;

namespace MultitenancyApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly TenantDbContext _context;

        public ProductRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            return product;
        }
        
        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}