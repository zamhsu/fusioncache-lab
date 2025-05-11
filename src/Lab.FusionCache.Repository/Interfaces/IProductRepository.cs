using Lab.FusionCache.Repository.Entities;

namespace Lab.FusionCache.Repository.Interfaces;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product);
    
    Task<List<Product>> GetAllAsync();
    
    Task<Product> GetByIdAsync(int id);
    
    Task<bool> UpdateAsync(Product product);
    
    Task<bool> DeleteAsync(int id);
}