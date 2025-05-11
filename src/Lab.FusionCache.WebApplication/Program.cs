using Microsoft.EntityFrameworkCore;
using Lab.FusionCache.Repository.DbContexts;
using Lab.FusionCache.Repository.Implements;
using Lab.FusionCache.Repository.Interfaces;
using Lab.FusionCache.WebApplication.Apis;
using Lab.FusionCache.WebApplication.Infrastructure.ServiceCollectionExtensions;
using Lab.FusionCache.Service.Implements;
using Lab.FusionCache.Service.Interfaces;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.NeueccMessagePack;

var builder = WebApplication.CreateBuilder(args);

// Add FusionCache (這個要放在前面)
builder.Services.AddFusionCache()
    .WithOptions(options =>
    {
        options.CacheKeyPrefix = "Test.FusionCache:";
        options.DistributedCacheCircuitBreakerDuration = TimeSpan.FromMilliseconds(500);

        // 共用預設值
        options.DefaultEntryOptions.Duration = TimeSpan.FromMinutes(1);
        options.DefaultEntryOptions.DistributedCacheDuration = TimeSpan.FromHours(1);
        options.DefaultEntryOptions.DistributedCacheSoftTimeout = TimeSpan.FromMilliseconds(10);
        options.DefaultEntryOptions.DistributedCacheHardTimeout = TimeSpan.FromMilliseconds(500);
        options.DefaultEntryOptions.AllowBackgroundDistributedCacheOperations = true;
        options.DefaultEntryOptions.JitterMaxDuration = TimeSpan.FromSeconds(30);
    })
    .WithSerializer(
        new FusionCacheNeueccMessagePackSerializer()
    )
    .WithDistributedCache(
        new RedisCache(new RedisCacheOptions
        {
            Configuration = "localhost:6379"
        })
    )
    .WithBackplane(
        new RedisBackplane(new RedisBackplaneOptions { Configuration = "localhost:6379" })
    );

// Add services to the container.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddDbContext<EcShopContext>(options =>
    options.UseInMemoryDatabase("EcShop"));

// Add demo data.
builder.Services.AddDemoData();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/api/product")
    .WithTags("Product API")
    .MapProductApi();

app.Run();