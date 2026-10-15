using System.Net.Http.Json;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Mvc.Services.Http;

/// <summary>
/// Implementação de IPostService que consome a Vanderluiz.Hub.Api via HTTP.
/// </summary>
public class PostHttpService : IPostService
{
    private readonly HttpClient _http;

    public PostHttpService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<PostResumoDto>> ListarPublicadosAsync(int? quantidade = null, CancellationToken ct = default)
    {
        var url = $"/api/posts{(quantidade.HasValue ? $"?quantidade={quantidade.Value}" : "")}";
        var resposta = await _http.GetFromJsonAsync<IReadOnlyList<PostResumoDto>>(url, ct);
        return resposta ?? [];
    }

    public async Task<PostDto?> ObterPublicadoPorSlugAsync(string slug, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/posts/{slug}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PostDto>(cancellationToken: ct);
    }
}

