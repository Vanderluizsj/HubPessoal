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

    public async Task<IReadOnlyList<FormacaoDto>> ListarFormacoesAsync(CancellationToken ct = default)
    {
        var resposta = await _http.GetFromJsonAsync<IReadOnlyList<FormacaoDto>>("/api/trajetoria/formacoes", ct);
        return resposta ?? [];
    }
}

