using System.Net.Http.Json;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Mvc.Services.Http;

/// <summary>
/// Implementação de ITecnologiaService que consome a Vanderluiz.Hub.Api via HTTP.
/// </summary>
public class TecnologiaHttpService : ITecnologiaService
{
    private readonly HttpClient _http;

    public TecnologiaHttpService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<TecnologiaDto>> ListarAsync(CancellationToken ct = default)
    {
        var resposta = await _http.GetFromJsonAsync<IReadOnlyList<TecnologiaDto>>("/api/tecnologias", ct);
        return resposta ?? [];
    }

    public async Task<TecnologiaDto?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/tecnologias/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<TecnologiaDto>(cancellationToken: ct);
    }

    public async Task<TecnologiaDto> CriarAsync(TecnologiaInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PostAsJsonAsync("/api/tecnologias", input, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<TecnologiaDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar tecnologia na API.");
    }

    public async Task<bool> AtualizarAsync(int id, TecnologiaInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PutAsJsonAsync($"/api/tecnologias/{id}", input, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.DeleteAsync($"/api/tecnologias/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }
}
