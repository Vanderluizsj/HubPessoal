using System.Net.Http.Json;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Mvc.Services.Http;

/// <summary>
/// Implementação de ITrajetoriaService que consome a Vanderluiz.Hub.Api via HTTP.
/// </summary>
public class TrajetoriaHttpService : ITrajetoriaService
{
    private readonly HttpClient _http;

    public TrajetoriaHttpService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ExperienciaDto>> ListarExperienciasAsync(CancellationToken ct = default)
    {
        var resposta = await _http.GetFromJsonAsync<IReadOnlyList<ExperienciaDto>>("/api/trajetoria/experiencias", ct);
        return resposta ?? [];
    }

    public async Task<ExperienciaDto?> ObterExperienciaPorIdAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/trajetoria/experiencias/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ExperienciaDto>(cancellationToken: ct);
    }

    public async Task<ExperienciaDto> CriarExperienciaAsync(ExperienciaInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PostAsJsonAsync("/api/trajetoria/experiencias", input, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ExperienciaDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar experiência via API.");
    }

    public async Task<bool> AtualizarExperienciaAsync(int id, ExperienciaInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PutAsJsonAsync($"/api/trajetoria/experiencias/{id}", input, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverExperienciaAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.DeleteAsync($"/api/trajetoria/experiencias/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<IReadOnlyList<FormacaoDto>> ListarFormacoesAsync(CancellationToken ct = default)
    {
        var resposta = await _http.GetFromJsonAsync<IReadOnlyList<FormacaoDto>>("/api/trajetoria/formacoes", ct);
        return resposta ?? [];
    }

    public async Task<FormacaoDto?> ObterFormacaoPorIdAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/trajetoria/formacoes/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<FormacaoDto>(cancellationToken: ct);
    }

    public async Task<FormacaoDto> CriarFormacaoAsync(FormacaoInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PostAsJsonAsync("/api/trajetoria/formacoes", input, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<FormacaoDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar formação via API.");
    }

    public async Task<bool> AtualizarFormacaoAsync(int id, FormacaoInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PutAsJsonAsync($"/api/trajetoria/formacoes/{id}", input, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverFormacaoAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.DeleteAsync($"/api/trajetoria/formacoes/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }
}

