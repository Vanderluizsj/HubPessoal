using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Infrastructure.Data;
using Vanderluiz.Hub.Infrastructure.Repositories;

namespace Vanderluiz.Hub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<HubDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(HubDbContext).Assembly.FullName)));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProjetoRepository, ProjetoRepository>();
        services.AddScoped<ITecnologiaRepository, TecnologiaRepository>();
        services.AddScoped<IPostRepository, PostRepository>();

        services.AddScoped<DbInitializer>();
        return services;
    }
}

