using Microsoft.EntityFrameworkCore;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Domain.Entities;
using Vanderluiz.Hub.Infrastructure.Data;

namespace Vanderluiz.Hub.Infrastructure.Repositories;

public class ProjetoRepository : Repository<Projeto>, IProjetoRepository
{
    public ProjetoRepository(HubDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Projeto>> ListarComTecnologiasAsync(bool somenteDestaques, CancellationToken ct = default)
    {
        var query = Set.AsNoTracking().Include(p => p.Tecnologias).AsQueryable();
        if (somenteDestaques) query = query.Where(p => p.Destaque);

        return await query
            .OrderBy(p => p.Ordem)
            .ThenByDescending(p => p.DataInicio)
            .ToListAsync(ct);
    }

    public Task<Projeto?> ObterPorSlugAsync(string slug, CancellationToken ct = default) =>
        Set.AsNoTracking().Include(p => p.Tecnologias).FirstOrDefaultAsync(p => p.Slug == slug, ct);

    public Task<Projeto?> ObterComTecnologiasAsync(int id, CancellationToken ct = default) =>
        Set.Include(p => p.Tecnologias).FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<bool> SlugExisteAsync(string slug, int? ignorarId = null, CancellationToken ct = default) =>
        Set.AnyAsync(p => p.Slug == slug && (ignorarId == null || p.Id != ignorarId), ct);
}

public class TecnologiaRepository : Repository<Tecnologia>, ITecnologiaRepository
{
    public TecnologiaRepository(HubDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Tecnologia>> ListarComProjetosAsync(CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Include(t => t.Projetos)
            .OrderBy(t => t.Categoria).ThenBy(t => t.Ordem).ThenBy(t => t.Nome)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Tecnologia>> ListarPorIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var lista = ids.Distinct().ToList();
        return await Set.Where(t => lista.Contains(t.Id)).ToListAsync(ct);
    }
}

public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(HubDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Post>> ListarPublicadosAsync(int? quantidade, CancellationToken ct = default)
    {
        var query = Set.AsNoTracking()
            .Where(p => p.Publicado)
            .OrderByDescending(p => p.PublicadoEm)
            .AsQueryable();

        if (quantidade is > 0) query = query.Take(quantidade.Value);
        return await query.ToListAsync(ct);
    }

    public Task<Post?> ObterPublicadoPorSlugAsync(string slug, CancellationToken ct = default) =>
        Set.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug && p.Publicado, ct);
}

