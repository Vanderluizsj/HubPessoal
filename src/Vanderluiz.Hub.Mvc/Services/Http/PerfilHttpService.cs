using System.Net.Http.Json;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Mvc.Services.Http;

/// <summary>
/// Consome as informações de Perfil e Contato da Vanderluiz.Hub.Api.
/// </summary>
public class PerfilHttpService : IPerfilService
{
    private readonly HttpClient _http;

    public PerfilHttpService(HttpClient http)
    {
        _http = http;
    }

    public async Task<PerfilDto> ObterAsync(CancellationToken ct = default)
    {
        var perfil = await _http.GetFromJsonAsync<PerfilDto>("/api/perfil", ct);
        return perfil ?? new PerfilDto(0, "Vanderluiz", "Desenvolvedor .NET", "Bio", "Brasil", null, null, null, null, null);
    }

    public async Task<PerfilDto> AtualizarAsync(PerfilInputDto input, CancellationToken ct = default)
    {
        var resposta = await _http.PutAsJsonAsync("/api/perfil", input, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PerfilDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao atualizar perfil via API.");
    }
}

