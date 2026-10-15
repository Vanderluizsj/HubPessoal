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

    public async Task<ExperienciaDto?> ObterExperienciaPorIdAsync(int id, CancellationToken ct = default)
    {
        var item = await _experiencias.ObterPorIdAsync(id, ct);
        return item?.ToDto();
    }

    public async Task<ExperienciaDto> CriarExperienciaAsync(ExperienciaInputDto input, CancellationToken ct = default)
    {
        var experiencia = new Experiencia
        {
            Cargo = input.Cargo.Trim(),
            Empresa = input.Empresa.Trim(),
            Local = string.IsNullOrWhiteSpace(input.Local) ? null : input.Local.Trim(),
            Descricao = input.Descricao.Trim(),
            DataInicio = input.DataInicio,
            DataFim = input.DataFim
        };

        await _experiencias.AdicionarAsync(experiencia, ct);
        await _experiencias.SalvarAsync(ct);

        return experiencia.ToDto();
    }

    public async Task<bool> AtualizarExperienciaAsync(int id, ExperienciaInputDto input, CancellationToken ct = default)
    {
        var experiencia = await _experiencias.ObterPorIdAsync(id, ct);
        if (experiencia is null) return false;

        experiencia.Cargo = input.Cargo.Trim();
        experiencia.Empresa = input.Empresa.Trim();
        experiencia.Local = string.IsNullOrWhiteSpace(input.Local) ? null : input.Local.Trim();
        experiencia.Descricao = input.Descricao.Trim();
        experiencia.DataInicio = input.DataInicio;
        experiencia.DataFim = input.DataFim;
        experiencia.AtualizadoEm = DateTime.UtcNow;

        _experiencias.Atualizar(experiencia);
        await _experiencias.SalvarAsync(ct);
        return true;
    }

    public async Task<bool> RemoverExperienciaAsync(int id, CancellationToken ct = default)
    {
        var experiencia = await _experiencias.ObterPorIdAsync(id, ct);
        if (experiencia is null) return false;

        _experiencias.Remover(experiencia);
        await _experiencias.SalvarAsync(ct);
        return true;
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

    public async Task<FormacaoDto?> ObterFormacaoPorIdAsync(int id, CancellationToken ct = default)
    {
        var item = await _formacoes.ObterPorIdAsync(id, ct);
        return item?.ToDto();
    }

    public async Task<FormacaoDto> CriarFormacaoAsync(FormacaoInputDto input, CancellationToken ct = default)
    {
        var formacao = new Formacao
        {
            Curso = input.Curso.Trim(),
            Instituicao = input.Instituicao.Trim(),
            Tipo = input.Tipo,
            Descricao = string.IsNullOrWhiteSpace(input.Descricao) ? null : input.Descricao.Trim(),
            DataInicio = input.DataInicio,
            DataFim = input.DataFim
        };

        await _formacoes.AdicionarAsync(formacao, ct);
        await _formacoes.SalvarAsync(ct);

        return formacao.ToDto();
    }

    public async Task<bool> AtualizarFormacaoAsync(int id, FormacaoInputDto input, CancellationToken ct = default)
    {
        var formacao = await _formacoes.ObterPorIdAsync(id, ct);
        if (formacao is null) return false;

        formacao.Curso = input.Curso.Trim();
        formacao.Instituicao = input.Instituicao.Trim();
        formacao.Tipo = input.Tipo;
        formacao.Descricao = string.IsNullOrWhiteSpace(input.Descricao) ? null : input.Descricao.Trim();
        formacao.DataInicio = input.DataInicio;
        formacao.DataFim = input.DataFim;
        formacao.AtualizadoEm = DateTime.UtcNow;

        _formacoes.Atualizar(formacao);
        await _formacoes.SalvarAsync(ct);
        return true;
    }

    public async Task<bool> RemoverFormacaoAsync(int id, CancellationToken ct = default)
    {
        var formacao = await _formacoes.ObterPorIdAsync(id, ct);
        if (formacao is null) return false;

        _formacoes.Remover(formacao);
        await _formacoes.SalvarAsync(ct);
        return true;
    }
}

