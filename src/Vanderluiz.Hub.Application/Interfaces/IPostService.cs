using Vanderluiz.Hub.Application.DTOs;

namespace Vanderluiz.Hub.Application.Interfaces;

public interface IPostService
{
    Task<IReadOnlyList<PostResumoDto>> ListarPublicadosAsync(int? quantidade = null, CancellationToken ct = default);
    Task<PostDto?> ObterPublicadoPorSlugAsync(string slug, CancellationToken ct = default);
}

