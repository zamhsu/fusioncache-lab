using Lab.FusionCache.WebApplication.Infrastructure.InputParameters;
using Lab.FusionCache.WebApplication.Infrastructure.OutputModels;
using Mapster;
using Lab.FusionCache.Service.Dtos;
using Lab.FusionCache.Service.Interfaces;

namespace Lab.FusionCache.WebApplication.Apis;

public static class ProductApi
{
    public static IEndpointRouteBuilder MapProductApi(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", CreateProductAsync);
        app.MapGet("/", GetAllProductsAsync);
        app.MapGet("/{id:int}", GetProductByIdAsync);
        app.MapPut("/{id:int}", UpdateProductAsync);
        app.MapDelete("/{id:int}", DeleteProductAsync);

        return app;
    }
    
    public static async Task<IResult> CreateProductAsync(IProductService service, CreateProductParameter parameter)
    {
        var dto = parameter.Adapt<CreateProductDto>();
        var result = await service.CreateAsync(dto);
        return TypedResults.Created($"/products/{result.Id}", result);
    }

    public static async Task<IResult> GetAllProductsAsync(IProductService service)
    {
        var dto = await service.GetAllAsync();
        var output = dto.Adapt<List<ProductOutputModel>>();
        return TypedResults.Ok(output);
    }

    public static async Task<IResult> GetProductByIdAsync(int id, IProductService service)
    {
        var dto = await service.GetByIdAsync(id);
        if (dto == null) return TypedResults.NotFound();

        var output = dto.Adapt<ProductOutputModel>();
        return TypedResults.Ok(output);
    }

    public static async Task<IResult> UpdateProductAsync(int id, UpdateProductParameter parameter, IProductService service)
    {
        var dto = parameter.Adapt<UpdateProductDto>();
        var success = await service.UpdateAsync(id, dto);
        return success ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<IResult> DeleteProductAsync(int id, IProductService service)
    {
        var success = await service.DeleteAsync(id);
        return success ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}