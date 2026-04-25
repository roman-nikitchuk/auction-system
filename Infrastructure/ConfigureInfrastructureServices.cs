using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureInfrastructureServices
{
    public static void AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<AppDbContextInitializer>();

        // репозиторії
        services.AddScoped<IRepository<User>, UserRepository>();
        services.AddScoped<IUserQueries, UserRepository>();

        services.AddScoped<IRepository<Auction>, AuctionRepository>();
        services.AddScoped<IAuctionQueries, AuctionRepository>();

        services.AddScoped<IRepository<Bid>, BidRepository>();
        services.AddScoped<IBidQueries, BidRepository>();

        services.AddScoped<IRepository<Category>, CategoryRepository>();
        services.AddScoped<ICategoryQueries, CategoryRepository>();
    }
}