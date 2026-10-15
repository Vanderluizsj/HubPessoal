using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Application.Mappings;
using Vanderluiz.Hub.Domain.Entities;

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

    public async Task<TecnologiaDto?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var tec = await _tecnologias.ObterPorIdAsync(id, ct);
        return tec?.ToDto();
    }

    public async Task<TecnologiaDto> CriarAsync(TecnologiaInputDto input, CancellationToken ct = default)
    {
        var nova = new Tecnologia
        {
            Nome = input.Nome.Trim(),
            Categoria = input.Categoria,
            Nivel = input.Nivel,
            IconeClasse = input.IconeClasse?.Trim(),
            Ordem = input.Ordem
        };

        await _tecnologias.AdicionarAsync(nova, ct);
        await _tecnologias.SalvarAsync(ct);
        return nova.ToDto();
    }

    public async Task<bool> AtualizarAsync(int id, TecnologiaInputDto input, CancellationToken ct = default)
    {
        var tec = await _tecnologias.ObterPorIdAsync(id, ct);
        if (tec is null) return false;

        tec.Nome = input.Nome.Trim();
        tec.Categoria = input.Categoria;
        tec.Nivel = input.Nivel;
        tec.IconeClasse = input.IconeClasse?.Trim();
        tec.Ordem = input.Ordem;
        tec.AtualizadoEm = DateTime.UtcNow;

        _tecnologias.Atualizar(tec);
        await _tecnologias.SalvarAsync(ct);
        return true;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var tec = await _tecnologias.ObterPorIdAsync(id, ct);
        if (tec is null) return false;

        _tecnologias.Remover(tec);
        await _tecnologias.SalvarAsync(ct);
        return true;
    }
}
