using Lab.FusionCache.Repository.Entities;
using Lab.FusionCache.Repository.Interfaces;

namespace Lab.FusionCache.WebApplication.Infrastructure.ServiceCollectionExtensions;

public static class DemoDataServiceCollectionExtension
{
    public static IServiceCollection AddDemoData(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

        // Add sample products
        var sampleProducts = new List<Product>
        {
            new Product { Title = "Product A", Quantity = 10, Price = 100, Description = "Description for Product A", CreateDate = DateTime.Now, UpdateDate = DateTime.Now },
            new Product { Title = "Product B", Quantity = 20, Price = 200, Description = "Description for Product B", CreateDate = DateTime.Now, UpdateDate = DateTime.Now },
            new Product { Title = "Product C", Quantity = 30, Price = 300, Description = "Description for Product C", CreateDate = DateTime.Now, UpdateDate = DateTime.Now }
        };

        foreach (var product in sampleProducts)
        {
            productRepository.CreateAsync(product).Wait();
        }
        
        return services;
    }
}