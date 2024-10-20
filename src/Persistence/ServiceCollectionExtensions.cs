using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Money.Map.Persistence.Repositories.Concrete;
using Persistence.Contexts;
using Persistence.Repositories.Interface;

namespace Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUow, Uow>();
        services.AddScoped<ContextSeedService>();
        
        string connectionString = configuration["ConnectionStrings:DefaultConnection"] ?? String.Empty;
        services.AddDbContextFactory<BaseDbContext>(opt => opt.UseNpgsql(connectionString), ServiceLifetime.Scoped);
        
        return services;
    }
}