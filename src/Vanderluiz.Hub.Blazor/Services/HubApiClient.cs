using System.Net.Http.Json;
using Vanderluiz.Hub.Application.DTOs;

namespace Vanderluiz.Hub.Blazor.Services;

/// <summary>
/// Cliente centralizado para consumo da Vanderluiz.Hub.Api pelo painel Blazor.
/// </summary>
public class HubApiClient
{
    private readonly HttpClient _http;

    public HubApiClient(HttpClient http)
    {
        _http = http;
    }

    // ==========================================
    // PROJETOS
    // ==========================================
    public async Task<IReadOnlyList<ProjetoResumoDto>> ListarProjetosAsync(bool somenteDestaques = false, CancellationToken ct = default)
    {
        var url = $"/api/projetos{(somenteDestaques ? "?somenteDestaques=true" : "")}";
        return await _http.GetFromJsonAsync<IReadOnlyList<ProjetoResumoDto>>(url, ct) ?? [];
    }

    public async Task<ProjetoDetalheDto?> ObterProjetoPorIdAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/projetos/id/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ProjetoDetalheDto>(cancellationToken: ct);
    }

    public async Task<ProjetoDetalheDto?> ObterProjetoPorSlugAsync(string slug, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/projetos/{slug}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ProjetoDetalheDto>(cancellationToken: ct);
    }

    public async Task<ProjetoDetalheDto> CriarProjetoAsync(ProjetoInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PostAsJsonAsync("/api/projetos", input, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ProjetoDetalheDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar projeto na API.");
    }

    public async Task<bool> AtualizarProjetoAsync(int id, ProjetoInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PutAsJsonAsync($"/api/projetos/{id}", input, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverProjetoAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.DeleteAsync($"/api/projetos/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // TECNOLOGIAS
    // ==========================================
    public async Task<IReadOnlyList<TecnologiaDto>> ListarTecnologiasAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<TecnologiaDto>>("/api/tecnologias", ct) ?? [];
    }

    // ==========================================
    // POSTS
    // ==========================================
    public async Task<IReadOnlyList<PostResumoDto>> ListarPostsAsync(int? quantidade = null, CancellationToken ct = default)
    {
        var url = $"/api/posts{(quantidade.HasValue ? $"?quantidade={quantidade.Value}" : "")}";
        return await _http.GetFromJsonAsync<IReadOnlyList<PostResumoDto>>(url, ct) ?? [];
    }

    // ==========================================
    // TRAJETÓRIA
    // ==========================================
    public async Task<IReadOnlyList<ExperienciaDto>> ListarExperienciasAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<ExperienciaDto>>("/api/trajetoria/experiencias", ct) ?? [];
    }

    public async Task<IReadOnlyList<FormacaoDto>> ListarFormacoesAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<FormacaoDto>>("/api/trajetoria/formacoes", ct) ?? [];
    }
}

