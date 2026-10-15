using System.Net.Http.Json;
using Vanderluiz.Hub.Application.DTOs;

namespace Vanderluiz.Hub.Blazor.Services;

/// <summary>
/// Cliente centralizado para consumo da Vanderluiz.Hub.Api pelo painel Blazor.
/// Envia o header X-Admin-Key automaticamente quando o usuário está autenticado como administrador.
/// </summary>
public class HubApiClient
{
    private readonly HttpClient _http;
    private readonly AdminStateService _adminState;

    public HubApiClient(HttpClient http, AdminStateService adminState)
    {
        _http = http;
        _adminState = adminState;
    }

    private HttpRequestMessage CriarRequisicao(HttpMethod metodo, string url, object? corpo = null)
    {
        var req = new HttpRequestMessage(metodo, url);
        if (_adminState.EstaAutenticado && !string.IsNullOrEmpty(_adminState.ChaveAdmin))
        {
            req.Headers.Add("X-Admin-Key", _adminState.ChaveAdmin);
        }

        if (corpo is not null)
        {
            req.Content = JsonContent.Create(corpo);
        }

        return req;
    }

    // ==========================================
    // AUTENTICAÇÃO
    // ==========================================
    public async Task<bool> ValidarLoginAsync(string chave, CancellationToken ct = default)
    {
        try
        {
            var resposta = await _http.PostAsJsonAsync("/api/auth/login", new { chave }, ct);
            return resposta.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
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

    public async Task<ProjetoDetalheDto> CriarProjetoAsync(ProjetoInputDto input, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Post, "/api/projetos", input);
        using var resposta = await _http.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ProjetoDetalheDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar projeto na API.");
    }

    public async Task<bool> AtualizarProjetoAsync(int id, ProjetoInputDto input, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Put, $"/api/projetos/{id}", input);
        using var resposta = await _http.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverProjetoAsync(int id, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Delete, $"/api/projetos/{id}");
        using var resposta = await _http.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // TECNOLOGIAS (CRUD COMPLETO)
    // ==========================================
    public async Task<IReadOnlyList<TecnologiaDto>> ListarTecnologiasAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<TecnologiaDto>>("/api/tecnologias", ct) ?? [];
    }

    public async Task<TecnologiaDto?> ObterTecnologiaPorIdAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/tecnologias/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<TecnologiaDto>(cancellationToken: ct);
    }

    public async Task<TecnologiaDto> CriarTecnologiaAsync(TecnologiaInputDto input, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Post, "/api/tecnologias", input);
        using var resposta = await _http.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<TecnologiaDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar tecnologia na API.");
    }

    public async Task<bool> AtualizarTecnologiaAsync(int id, TecnologiaInputDto input, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Put, $"/api/tecnologias/{id}", input);
        using var resposta = await _http.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverTecnologiaAsync(int id, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Delete, $"/api/tecnologias/{id}");
        using var resposta = await _http.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // POSTS & BLOG (CRUD COMPLETO)
    // ==========================================
    public async Task<IReadOnlyList<PostResumoDto>> ListarPostsAsync(int? quantidade = null, bool todos = true, CancellationToken ct = default)
    {
        var url = $"/api/posts?todos={todos}{(quantidade.HasValue ? $"&quantidade={quantidade.Value}" : "")}";
        return await _http.GetFromJsonAsync<IReadOnlyList<PostResumoDto>>(url, ct) ?? [];
    }

    public async Task<PostDto?> ObterPostPorIdAsync(int id, CancellationToken ct = default)
    {
        var resposta = await _http.GetAsync($"/api/posts/id/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PostDto>(cancellationToken: ct);
    }

    public async Task<PostDto> CriarPostAsync(PostInputDto input, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Post, "/api/posts", input);
        using var resposta = await _http.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PostDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar post na API.");
    }

    public async Task<bool> AtualizarPostAsync(int id, PostInputDto input, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Put, $"/api/posts/{id}", input);
        using var resposta = await _http.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverPostAsync(int id, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Delete, $"/api/posts/{id}");
        using var resposta = await _http.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // TRAJETÓRIA - EXPERIÊNCIAS
    // ==========================================
    public async Task<IReadOnlyList<ExperienciaDto>> ListarExperienciasAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<ExperienciaDto>>("/api/trajetoria/experiencias", ct) ?? [];
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
        using var req = CriarRequisicao(HttpMethod.Post, "/api/trajetoria/experiencias", input);
        using var resposta = await _http.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ExperienciaDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar experiência na API.");
    }

    public async Task<bool> AtualizarExperienciaAsync(int id, ExperienciaInputDto input, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Put, $"/api/trajetoria/experiencias/{id}", input);
        using var resposta = await _http.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverExperienciaAsync(int id, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Delete, $"/api/trajetoria/experiencias/{id}");
        using var resposta = await _http.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // TRAJETÓRIA - FORMAÇÕES
    // ==========================================
    public async Task<IReadOnlyList<FormacaoDto>> ListarFormacoesAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<FormacaoDto>>("/api/trajetoria/formacoes", ct) ?? [];
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
        using var req = CriarRequisicao(HttpMethod.Post, "/api/trajetoria/formacoes", input);
        using var resposta = await _http.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<FormacaoDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar formação na API.");
    }

    public async Task<bool> AtualizarFormacaoAsync(int id, FormacaoInputDto input, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Put, $"/api/trajetoria/formacoes/{id}", input);
        using var resposta = await _http.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverFormacaoAsync(int id, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Delete, $"/api/trajetoria/formacoes/{id}");
        using var resposta = await _http.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // PERFIL & CONTATO
    // ==========================================
    public async Task<PerfilDto?> ObterPerfilAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<PerfilDto>("/api/perfil", ct);
    }

    public async Task<PerfilDto> AtualizarPerfilAsync(PerfilInputDto input, CancellationToken ct = default)
    {
        using var req = CriarRequisicao(HttpMethod.Put, "/api/perfil", input);
        using var resposta = await _http.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PerfilDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao atualizar perfil na API.");
    }
}
