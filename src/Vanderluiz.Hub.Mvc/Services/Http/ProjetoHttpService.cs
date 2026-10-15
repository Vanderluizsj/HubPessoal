using System.Net.Http.Json;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Mvc.Services.Http;

/// <summary>
/// Implementação de IProjetoService que consome a Vanderluiz.Hub.Api via HTTP.
/// </summary>
public class ProjetoHttpService : IProjetoService
{
    private readonly HttpClient _http;

    public ProjetoHttpService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ProjetoResumoDto>> ListarAsync(bool somenteDestaques = false, CancellationToken ct = default)
    {
        var url = $"/api/projetos{(somenteDestaques ? "?somenteDestaques=true" : "")}";
        var resposta = await _http.GetFromJsonAsync<IReadOnlyList<ProjetoResumoDto>>(url, ct);
        return resposta ?? [];
    }

    public async Task<ProjetoDetalheDto?> ObterPorSlugAsync(string slug, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/projetos/{slug}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ProjetoDetalheDto>(cancellationToken: ct);
    }

    public async Task<ProjetoDetalheDto?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/projetos/id/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ProjetoDetalheDto>(cancellationToken: ct);
    }

    public async Task<ProjetoDetalheDto> CriarAsync(ProjetoInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PostAsJsonAsync("/api/projetos", input, ct);
        resposta.EnsureSuccessStatusCode();
        var criado = await resposta.Content.ReadFromJsonAsync<ProjetoDetalheDto>(cancellationToken: ct);
        return criado ?? throw new InvalidOperationException("Falha ao desserializar retorno da API.");
    }

    public async Task<bool> AtualizarAsync(int id, ProjetoInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PutAsJsonAsync($"/api/projetos/{id}", input, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.DeleteAsync($"/api/projetos/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }
}

