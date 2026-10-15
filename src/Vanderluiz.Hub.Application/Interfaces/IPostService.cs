using Vanderluiz.Hub.Application.DTOs;

namespace Vanderluiz.Hub.Application.Interfaces;

public interface IPostService
{
    Task<IReadOnlyList<PostResumoDto>> ListarPublicadosAsync(int? quantidade = null, CancellationToken ct = default);
    Task<IReadOnlyList<PostResumoDto>> ListarTodosAsync(CancellationToken ct = default);
    Task<PostDto?> ObterPublicadoPorSlugAsync(string slug, CancellationToken ct = default);
    Task<PostDto?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<PostDto> CriarAsync(PostInputDto input, CancellationToken ct = default);
    Task<bool> AtualizarAsync(int id, PostInputDto input, CancellationToken ct = default);
    Task<bool> RemoverAsync(int id, CancellationToken ct = default);
}
