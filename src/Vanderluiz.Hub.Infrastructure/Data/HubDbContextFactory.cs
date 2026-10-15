using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Vanderluiz.Hub.Infrastructure.Data;

namespace Vanderluiz.Hub.Infrastructure.Data;

/// <summary>
/// Fábrica em tempo de design para permitir que a ferramenta 'dotnet ef' crie migrações
/// sem depender do estado de runtime de outros projetos.
/// </summary>
public class HubDbContextFactory : IDesignTimeDbContextFactory<HubDbContext>
{
    public HubDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HubDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=VanderluizHub;User Id=sa;Password=Prov@ndd11;TrustServerCertificate=True;");
        return new HubDbContext(optionsBuilder.Options);
    }
}

