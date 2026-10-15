using Vanderluiz.Hub.Application.DTOs;

namespace Vanderluiz.Hub.Application.Interfaces;

public interface IPerfilService
{
    Task<PerfilDto> ObterAsync(CancellationToken ct = default);
    Task<PerfilDto> AtualizarAsync(PerfilInputDto input, CancellationToken ct = default);
}

