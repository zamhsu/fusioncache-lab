using Lab.FusionCache.Repository.DbContexts;
using Lab.FusionCache.Repository.Entities;
using Lab.FusionCache.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZiggyCreatures.Caching.Fusion;

namespace Lab.FusionCache.Repository.Implements;

public class ProductRepository : IProductRepository
{
    private readonly EcShopContext _context;
    private readonly IFusionCache _fusionCache;
    private readonly ILogger _logger;

    public ProductRepository(EcShopContext context, IFusionCache fusionCache, ILoggerFactory loggerFactory)
    {
        _context = context;
        _fusionCache = fusionCache;
        _logger = loggerFactory.CreateLogger<ProductRepository>();
    }

    // 快取鍵生成：統一管理鍵的命名
    private string GetCacheKey(string prefix, string identifier = "")
    {
        return $"{prefix}:{identifier}";
    }

    // Create
    public async Task<Product> CreateAsync(Product product)
    {
        _context.Product.Add(product);
        await _context.SaveChangesAsync();

        // 建立快取
        await _fusionCache.SetAsync(
            GetCacheKey("Product", product.Id.ToString()),
            product,
            options =>
            {
                options.DistributedCacheDuration = TimeSpan.FromMinutes(10);
                options.DistributedCacheSoftTimeout = TimeSpan.FromMilliseconds(10);
                options.DistributedCacheHardTimeout = TimeSpan.FromMilliseconds(500);
                options.Duration = TimeSpan.FromSeconds(5);
            }
        );
        
        return product;
    }

    // Read - All
    public async Task<List<Product>> GetAllAsync()
    {
        var products = await _context.Product.ToListAsync();
        
        return products;
    }

    // Read - By Id
    public async Task<Product> GetByIdAsync(int id)
    {
        var product = await _fusionCache.GetOrSetAsync(
            GetCacheKey("Product", id.ToString()),
            async ct =>
            {
                _logger.LogError("Getting product {id} from the database.", id);
                return await _context.Product.FindAsync(id, ct);
            },
            options =>
            {
                // 客製化設定
                options.DistributedCacheDuration = TimeSpan.FromMinutes(10);
                options.DistributedCacheSoftTimeout = TimeSpan.FromMilliseconds(10);
                options.DistributedCacheHardTimeout = TimeSpan.FromMilliseconds(500);
                options.Duration = TimeSpan.FromSeconds(5);
            }
        );

        return product;
    }

    // Update
    public async Task<bool> UpdateAsync(Product product)
    {
        var existing = await _context.Product.FindAsync(product.Id);
        if (existing == null)
            return false;

        _context.Entry(existing).CurrentValues.SetValues(product);
        await _context.SaveChangesAsync();

        // 更新相關快取
        await _fusionCache.SetAsync(
            GetCacheKey("Product", product.Id.ToString()),
            product,
            options =>
            {
                options.DistributedCacheDuration = TimeSpan.FromMinutes(10);
                options.DistributedCacheSoftTimeout = TimeSpan.FromMilliseconds(10);
                options.DistributedCacheHardTimeout = TimeSpan.FromMilliseconds(500);
                options.Duration = TimeSpan.FromSeconds(5);
            }
        );

        return true;
    }

    // Delete
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Product.FindAsync(id);
        if (product == null) return false;

        _context.Product.Remove(product);
        await _context.SaveChangesAsync();

        // 刪除相關快取
        await _fusionCache.RemoveAsync(GetCacheKey("Product", id.ToString()));

        return true;
    }
}