using Vanderluiz.Hub.Domain.Entities;

namespace Vanderluiz.Hub.Application.Interfaces.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListarAsync(CancellationToken ct = default);
    Task<int> ContarAsync(CancellationToken ct = default);
    Task AdicionarAsync(T entidade, CancellationToken ct = default);
    void Atualizar(T entidade);
    void Remover(T entidade);
    Task<int> SalvarAsync(CancellationToken ct = default);
}

public interface IProjetoRepository : IRepository<Projeto>
{
    Task<IReadOnlyList<Projeto>> ListarComTecnologiasAsync(bool somenteDestaques, CancellationToken ct = default);
    Task<Projeto?> ObterPorSlugAsync(string slug, CancellationToken ct = default);
    Task<Projeto?> ObterComTecnologiasAsync(int id, CancellationToken ct = default);
    Task<bool> SlugExisteAsync(string slug, int? ignorarId = null, CancellationToken ct = default);
}

public interface ITecnologiaRepository : IRepository<Tecnologia>
{
    Task<IReadOnlyList<Tecnologia>> ListarComProjetosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Tecnologia>> ListarPorIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
}

public interface IPostRepository : IRepository<Post>
{
    Task<IReadOnlyList<Post>> ListarPublicadosAsync(int? quantidade, CancellationToken ct = default);
    Task<Post?> ObterPublicadoPorSlugAsync(string slug, CancellationToken ct = default);
}

