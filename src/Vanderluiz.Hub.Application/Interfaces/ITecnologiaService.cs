using Vanderluiz.Hub.Application.DTOs;

namespace Vanderluiz.Hub.Application.Interfaces;

public interface ITecnologiaService
{
    Task<IReadOnlyList<TecnologiaDto>> ListarAsync(CancellationToken ct = default);
    Task<TecnologiaDto?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<TecnologiaDto> CriarAsync(TecnologiaInputDto input, CancellationToken ct = default);
    Task<bool> AtualizarAsync(int id, TecnologiaInputDto input, CancellationToken ct = default);
    Task<bool> RemoverAsync(int id, CancellationToken ct = default);
}
