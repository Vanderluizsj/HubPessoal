using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Application.Mappings;

namespace Vanderluiz.Hub.Application.Services;

public class TecnologiaService : ITecnologiaService
{
    private readonly ITecnologiaRepository _tecnologias;

    public TecnologiaService(ITecnologiaRepository tecnologias) => _tecnologias = tecnologias;

    public async Task<IReadOnlyList<TecnologiaDto>> ListarAsync(CancellationToken ct = default)
    {
        var tecnologias = await _tecnologias.ListarComProjetosAsync(ct);
        return tecnologias.Select(t => t.ToDto()).ToList();
    }
}

