using Microsoft.Extensions.DependencyInjection;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Application.Services;

namespace Vanderluiz.Hub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjetoService, ProjetoService>();
        services.AddScoped<ITecnologiaService, TecnologiaService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ITrajetoriaService, TrajetoriaService>();
        return services;
    }
}

