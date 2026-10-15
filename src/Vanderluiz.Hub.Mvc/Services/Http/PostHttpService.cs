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

    public async Task<IReadOnlyList<PostResumoDto>> ListarTodosAsync(CancellationToken ct = default)
    {
        var resposta = await _http.GetFromJsonAsync<IReadOnlyList<PostResumoDto>>("/api/posts?todos=true", ct);
        return resposta ?? [];
    }

    public async Task<PostDto?> ObterPublicadoPorSlugAsync(string slug, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/posts/{slug}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PostDto>(cancellationToken: ct);
    }

    public async Task<PostDto?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/posts/id/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PostDto>(cancellationToken: ct);
    }

    public async Task<PostDto> CriarAsync(PostInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PostAsJsonAsync("/api/posts", input, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PostDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar post na API.");
    }

    public async Task<bool> AtualizarAsync(int id, PostInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PutAsJsonAsync($"/api/posts/{id}", input, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.DeleteAsync($"/api/posts/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }
}
