using Vanderluiz.Hub.Application.DTOs;

namespace Vanderluiz.Hub.Application.Interfaces;

public interface IProjetoService
{
    Task<IReadOnlyList<ProjetoResumoDto>> ListarAsync(bool somenteDestaques = false, CancellationToken ct = default);
    Task<ProjetoDetalheDto?> ObterPorSlugAsync(string slug, CancellationToken ct = default);
    Task<ProjetoDetalheDto?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<ProjetoDetalheDto> CriarAsync(ProjetoInputDto input, CancellationToken ct = default);
    Task<bool> AtualizarAsync(int id, ProjetoInputDto input, CancellationToken ct = default);
    Task<bool> RemoverAsync(int id, CancellationToken ct = default);
}

