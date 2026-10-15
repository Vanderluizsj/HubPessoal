using Vanderluiz.Hub.Application.DTOs;

namespace Vanderluiz.Hub.Application.Interfaces;

public interface ITecnologiaService
{
    Task<IReadOnlyList<TecnologiaDto>> ListarAsync(CancellationToken ct = default);
}

