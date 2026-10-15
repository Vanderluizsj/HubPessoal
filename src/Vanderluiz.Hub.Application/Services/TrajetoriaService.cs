using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Application.Mappings;
using Vanderluiz.Hub.Domain.Entities;

namespace Vanderluiz.Hub.Application.Services;

public class TrajetoriaService : ITrajetoriaService
{
    private readonly IRepository<Experiencia> _experiencias;
    private readonly IRepository<Formacao> _formacoes;

    public TrajetoriaService(IRepository<Experiencia> experiencias, IRepository<Formacao> formacoes)
    {
        _experiencias = experiencias;
        _formacoes = formacoes;
    }

    public async Task<IReadOnlyList<ExperienciaDto>> ListarExperienciasAsync(CancellationToken ct = default)
    {
        var itens = await _experiencias.ListarAsync(ct);
        return itens
            .OrderByDescending(e => e.DataFim is null)
            .ThenByDescending(e => e.DataInicio)
            .Select(e => e.ToDto())
            .ToList();
    }

    public async Task<IReadOnlyList<FormacaoDto>> ListarFormacoesAsync(CancellationToken ct = default)
    {
        var itens = await _formacoes.ListarAsync(ct);
        return itens
            .OrderByDescending(f => f.DataFim is null)
            .ThenByDescending(f => f.DataInicio)
            .Select(f => f.ToDto())
            .ToList();
    }
}

