using Microsoft.EntityFrameworkCore;
using Vanderluiz.Hub.Domain.Entities;

namespace Vanderluiz.Hub.Infrastructure.Data;

public class HubDbContext : DbContext
{
    public HubDbContext(DbContextOptions<HubDbContext> options) : base(options) { }

    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<Tecnologia> Tecnologias => Set<Tecnologia>();
    public DbSet<Experiencia> Experiencias => Set<Experiencia>();
    public DbSet<Formacao> Formacoes => Set<Formacao>();
    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HubDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

