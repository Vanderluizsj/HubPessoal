using Microsoft.EntityFrameworkCore;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Domain.Entities;
using Vanderluiz.Hub.Infrastructure.Data;

namespace Vanderluiz.Hub.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly HubDbContext Context;
    protected readonly DbSet<T> Set;

    public Repository(HubDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public virtual Task<T?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        Set.FirstOrDefaultAsync(e => e.Id == id, ct);

    public virtual async Task<IReadOnlyList<T>> ListarAsync(CancellationToken ct = default) =>
        await Set.AsNoTracking().ToListAsync(ct);

    public Task<int> ContarAsync(CancellationToken ct = default) => Set.CountAsync(ct);

    public async Task AdicionarAsync(T entidade, CancellationToken ct = default) =>
        await Set.AddAsync(entidade, ct);

    public void Atualizar(T entidade) => Set.Update(entidade);

    public void Remover(T entidade) => Set.Remove(entidade);

    public Task<int> SalvarAsync(CancellationToken ct = default) => Context.SaveChangesAsync(ct);
}

