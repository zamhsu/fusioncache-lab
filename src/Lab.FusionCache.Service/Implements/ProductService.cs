using Mapster;
using Lab.FusionCache.Repository.Entities;
using Lab.FusionCache.Repository.Interfaces;
using Lab.FusionCache.Service.Dtos;
using Lab.FusionCache.Service.Interfaces;

namespace Lab.FusionCache.Service.Implements;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // Create
    public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
    {
        var product = createProductDto.Adapt<Product>();
        var createdProduct = await _productRepository.CreateAsync(product);
        return createdProduct.Adapt<ProductDto>();
    }

    // Read - All
    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Adapt<List<ProductDto>>();
    }

    // Read - By Id
    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product.Adapt<ProductDto>();
    }

    // Update
    public async Task<bool> UpdateAsync(int id, UpdateProductDto updateProductDto)
    {
        var product = updateProductDto.Adapt<Product>();
        product.Id = id; // Ensure the ID is set
        return await _productRepository.UpdateAsync(product);
    }

    // Delete
    public async Task<bool> DeleteAsync(int id)
    {
        return await _productRepository.DeleteAsync(id);
    }
}