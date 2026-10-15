using Vanderluiz.Hub.Application.Common;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Application.Mappings;
using Vanderluiz.Hub.Domain.Entities;

namespace Vanderluiz.Hub.Application.Services;

public class ProjetoService : IProjetoService
{
    private readonly IProjetoRepository _projetos;
    private readonly ITecnologiaRepository _tecnologias;

    public ProjetoService(IProjetoRepository projetos, ITecnologiaRepository tecnologias)
    {
        _projetos = projetos;
        _tecnologias = tecnologias;
    }

    public async Task<IReadOnlyList<ProjetoResumoDto>> ListarAsync(bool somenteDestaques = false, CancellationToken ct = default)
    {
        var projetos = await _projetos.ListarComTecnologiasAsync(somenteDestaques, ct);
        return projetos.Select(p => p.ToResumoDto()).ToList();
    }

    public async Task<ProjetoDetalheDto?> ObterPorSlugAsync(string slug, CancellationToken ct = default)
    {
        var projeto = await _projetos.ObterPorSlugAsync(slug, ct);
        return projeto?.ToDetalheDto();
    }

    public async Task<ProjetoDetalheDto?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var projeto = await _projetos.ObterComTecnologiasAsync(id, ct);
        return projeto?.ToDetalheDto();
    }

    public async Task<ProjetoDetalheDto> CriarAsync(ProjetoInputDto input, CancellationToken ct = default)
    {
        var projeto = new Projeto();
        await AplicarAsync(projeto, input, null, ct);

        await _projetos.AdicionarAsync(projeto, ct);
        await _projetos.SalvarAsync(ct);
        return projeto.ToDetalheDto();
    }

    public async Task<bool> AtualizarAsync(int id, ProjetoInputDto input, CancellationToken ct = default)
    {
        var projeto = await _projetos.ObterComTecnologiasAsync(id, ct);
        if (projeto is null) return false;

        await AplicarAsync(projeto, input, id, ct);
        projeto.AtualizadoEm = DateTime.UtcNow;

        _projetos.Atualizar(projeto);
        await _projetos.SalvarAsync(ct);
        return true;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var projeto = await _projetos.ObterPorIdAsync(id, ct);
        if (projeto is null) return false;

        _projetos.Remover(projeto);
        await _projetos.SalvarAsync(ct);
        return true;
    }

    private async Task AplicarAsync(Projeto projeto, ProjetoInputDto input, int? idAtual, CancellationToken ct)
    {
        var slug = SlugHelper.Gerar(string.IsNullOrWhiteSpace(input.Slug) ? input.Titulo : input.Slug);
        if (await _projetos.SlugExisteAsync(slug, idAtual, ct))
            throw new InvalidOperationException($"Já existe um projeto com o slug '{slug}'.");

        projeto.Titulo = input.Titulo.Trim();
        projeto.Slug = slug;
        projeto.Resumo = input.Resumo;
        projeto.Descricao = input.Descricao;
        projeto.UrlRepositorio = input.UrlRepositorio;
        projeto.UrlDemo = input.UrlDemo;
        projeto.ImagemUrl = input.ImagemUrl;
        projeto.Status = input.Status;
        projeto.Destaque = input.Destaque;
        projeto.Ordem = input.Ordem;
        projeto.DataInicio = input.DataInicio;
        projeto.DataFim = input.DataFim;

        var tecnologias = await _tecnologias.ListarPorIdsAsync(input.TecnologiaIds, ct);
        projeto.Tecnologias.Clear();
        foreach (var t in tecnologias) projeto.Tecnologias.Add(t);
    }
}

