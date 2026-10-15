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
}

