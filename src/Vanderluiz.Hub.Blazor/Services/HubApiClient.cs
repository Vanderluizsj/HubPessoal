using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Blazor.Services;

/// <summary>
/// Cliente centralizado para consumo de dados pelo painel Blazor.
/// Suporta tanto o modo HTTP (quando conectado a uma API remota)
/// quanto o modo in-process (modo monólito unificado, sem portas adicionais).
/// </summary>
/// <remarks>
/// No modo in-process, cada chamada cria seu próprio escopo de DI (e portanto seu próprio DbContext).
/// Isso é necessário porque um circuito Blazor Server vive por toda a sessão do usuário e as páginas
/// disparam consultas em paralelo (Task.WhenAll) — o DbContext não suporta operações concorrentes.
/// </remarks>
public class HubApiClient
{
    private readonly HttpClient? _http;
    private readonly AdminStateService _adminState;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly IConfiguration? _configuration;
    private readonly string? _webRootPath;

    /// <summary>Modo HTTP: consome a Vanderluiz.Hub.Api remota.</summary>
    [ActivatorUtilitiesConstructor]
    public HubApiClient(HttpClient http, AdminStateService adminState)
    {
        _http = http;
        _adminState = adminState;
    }

    /// <summary>Modo in-process: usa diretamente os serviços da camada Application.</summary>
    public HubApiClient(
        AdminStateService adminState,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        string? webRootPath = null)
    {
        _adminState = adminState;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _webRootPath = webRootPath;
    }

    private bool EmProcesso => _scopeFactory is not null;

    private async Task<TResult> UsarServicoAsync<TService, TResult>(Func<TService, Task<TResult>> acao, bool exigeAdmin = false)
        where TService : notnull
    {
        if (exigeAdmin && !_adminState.EstaAutenticado)
        {
            throw new UnauthorizedAccessException("Operação restrita para administradores autenticados.");
        }

        await using var scope = _scopeFactory!.CreateAsyncScope();
        var servico = scope.ServiceProvider.GetRequiredService<TService>();
        return await acao(servico);
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
        if (EmProcesso)
        {
            var chaveEsperada = _configuration!["Admin:ChaveAcesso"];
            return !string.IsNullOrEmpty(chaveEsperada) && chave == chaveEsperada;
        }

        try
        {
            var resposta = await _http!.PostAsJsonAsync("/api/auth/login", new { chave }, ct);
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
        if (EmProcesso)
            return await UsarServicoAsync<IProjetoService, IReadOnlyList<ProjetoResumoDto>>(s => s.ListarAsync(somenteDestaques, ct));

        var url = $"/api/projetos{(somenteDestaques ? "?somenteDestaques=true" : "")}";
        return await _http!.GetFromJsonAsync<IReadOnlyList<ProjetoResumoDto>>(url, ct) ?? [];
    }

    public async Task<ProjetoDetalheDto?> ObterProjetoPorIdAsync(int id, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<IProjetoService, ProjetoDetalheDto?>(s => s.ObterPorIdAsync(id, ct));

        var resposta = await _http!.GetAsync($"/api/projetos/id/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ProjetoDetalheDto>(cancellationToken: ct);
    }

    public async Task<ProjetoDetalheDto> CriarProjetoAsync(ProjetoInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<IProjetoService, ProjetoDetalheDto>(s => s.CriarAsync(input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Post, "/api/projetos", input);
        using var resposta = await _http!.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ProjetoDetalheDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar projeto na API.");
    }

    public async Task<bool> AtualizarProjetoAsync(int id, ProjetoInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<IProjetoService, bool>(s => s.AtualizarAsync(id, input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Put, $"/api/projetos/{id}", input);
        using var resposta = await _http!.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverProjetoAsync(int id, CancellationToken ct = default)
    {
        if (EmProcesso)
        {
            var detalhe = await ObterProjetoPorIdAsync(id, ct);
            var removido = await UsarServicoAsync<IProjetoService, bool>(s => s.RemoverAsync(id, ct), exigeAdmin: true);
            if (removido && detalhe is not null)
            {
                RemoverArquivoLocalSeExistir(detalhe.ImagemUrl);
            }
            return removido;
        }

        using var req = CriarRequisicao(HttpMethod.Delete, $"/api/projetos/{id}");
        using var resposta = await _http!.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // TECNOLOGIAS (CRUD COMPLETO)
    // ==========================================
    public async Task<IReadOnlyList<TecnologiaDto>> ListarTecnologiasAsync(CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITecnologiaService, IReadOnlyList<TecnologiaDto>>(s => s.ListarAsync(ct));

        return await _http!.GetFromJsonAsync<IReadOnlyList<TecnologiaDto>>("/api/tecnologias", ct) ?? [];
    }

    public async Task<TecnologiaDto?> ObterTecnologiaPorIdAsync(int id, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITecnologiaService, TecnologiaDto?>(s => s.ObterPorIdAsync(id, ct));

        var resposta = await _http!.GetAsync($"/api/tecnologias/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<TecnologiaDto>(cancellationToken: ct);
    }

    public async Task<TecnologiaDto> CriarTecnologiaAsync(TecnologiaInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITecnologiaService, TecnologiaDto>(s => s.CriarAsync(input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Post, "/api/tecnologias", input);
        using var resposta = await _http!.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<TecnologiaDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar tecnologia na API.");
    }

    public async Task<bool> AtualizarTecnologiaAsync(int id, TecnologiaInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITecnologiaService, bool>(s => s.AtualizarAsync(id, input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Put, $"/api/tecnologias/{id}", input);
        using var resposta = await _http!.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverTecnologiaAsync(int id, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITecnologiaService, bool>(s => s.RemoverAsync(id, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Delete, $"/api/tecnologias/{id}");
        using var resposta = await _http!.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // POSTS & BLOG (CRUD COMPLETO)
    // ==========================================
    public async Task<IReadOnlyList<PostResumoDto>> ListarPostsAsync(int? quantidade = null, bool todos = true, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<IPostService, IReadOnlyList<PostResumoDto>>(s =>
                todos ? s.ListarTodosAsync(ct) : s.ListarPublicadosAsync(quantidade, ct));

        var url = $"/api/posts?todos={todos}{(quantidade.HasValue ? $"&quantidade={quantidade.Value}" : "")}";
        return await _http!.GetFromJsonAsync<IReadOnlyList<PostResumoDto>>(url, ct) ?? [];
    }

    public async Task<PostDto?> ObterPostPorIdAsync(int id, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<IPostService, PostDto?>(s => s.ObterPorIdAsync(id, ct));

        var resposta = await _http!.GetAsync($"/api/posts/id/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PostDto>(cancellationToken: ct);
    }

    public async Task<PostDto> CriarPostAsync(PostInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<IPostService, PostDto>(s => s.CriarAsync(input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Post, "/api/posts", input);
        using var resposta = await _http!.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PostDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar post na API.");
    }

    public async Task<bool> AtualizarPostAsync(int id, PostInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<IPostService, bool>(s => s.AtualizarAsync(id, input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Put, $"/api/posts/{id}", input);
        using var resposta = await _http!.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverPostAsync(int id, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<IPostService, bool>(s => s.RemoverAsync(id, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Delete, $"/api/posts/{id}");
        using var resposta = await _http!.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // TRAJETÓRIA - EXPERIÊNCIAS
    // ==========================================
    public async Task<IReadOnlyList<ExperienciaDto>> ListarExperienciasAsync(CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITrajetoriaService, IReadOnlyList<ExperienciaDto>>(s => s.ListarExperienciasAsync(ct));

        return await _http!.GetFromJsonAsync<IReadOnlyList<ExperienciaDto>>("/api/trajetoria/experiencias", ct) ?? [];
    }

    public async Task<ExperienciaDto?> ObterExperienciaPorIdAsync(int id, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITrajetoriaService, ExperienciaDto?>(s => s.ObterExperienciaPorIdAsync(id, ct));

        var resposta = await _http!.GetAsync($"/api/trajetoria/experiencias/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ExperienciaDto>(cancellationToken: ct);
    }

    public async Task<ExperienciaDto> CriarExperienciaAsync(ExperienciaInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITrajetoriaService, ExperienciaDto>(s => s.CriarExperienciaAsync(input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Post, "/api/trajetoria/experiencias", input);
        using var resposta = await _http!.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<ExperienciaDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar experiência na API.");
    }

    public async Task<bool> AtualizarExperienciaAsync(int id, ExperienciaInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITrajetoriaService, bool>(s => s.AtualizarExperienciaAsync(id, input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Put, $"/api/trajetoria/experiencias/{id}", input);
        using var resposta = await _http!.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverExperienciaAsync(int id, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITrajetoriaService, bool>(s => s.RemoverExperienciaAsync(id, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Delete, $"/api/trajetoria/experiencias/{id}");
        using var resposta = await _http!.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // TRAJETÓRIA - FORMAÇÕES
    // ==========================================
    public async Task<IReadOnlyList<FormacaoDto>> ListarFormacoesAsync(CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITrajetoriaService, IReadOnlyList<FormacaoDto>>(s => s.ListarFormacoesAsync(ct));

        return await _http!.GetFromJsonAsync<IReadOnlyList<FormacaoDto>>("/api/trajetoria/formacoes", ct) ?? [];
    }

    public async Task<FormacaoDto?> ObterFormacaoPorIdAsync(int id, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITrajetoriaService, FormacaoDto?>(s => s.ObterFormacaoPorIdAsync(id, ct));

        var resposta = await _http!.GetAsync($"/api/trajetoria/formacoes/{id}", ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<FormacaoDto>(cancellationToken: ct);
    }

    public async Task<FormacaoDto> CriarFormacaoAsync(FormacaoInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITrajetoriaService, FormacaoDto>(s => s.CriarFormacaoAsync(input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Post, "/api/trajetoria/formacoes", input);
        using var resposta = await _http!.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<FormacaoDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao criar formação na API.");
    }

    public async Task<bool> AtualizarFormacaoAsync(int id, FormacaoInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITrajetoriaService, bool>(s => s.AtualizarFormacaoAsync(id, input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Put, $"/api/trajetoria/formacoes/{id}", input);
        using var resposta = await _http!.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoverFormacaoAsync(int id, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<ITrajetoriaService, bool>(s => s.RemoverFormacaoAsync(id, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Delete, $"/api/trajetoria/formacoes/{id}");
        using var resposta = await _http!.SendAsync(req, ct);
        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        resposta.EnsureSuccessStatusCode();
        return true;
    }

    // ==========================================
    // PERFIL & CONTATO
    // ==========================================
    public async Task<PerfilDto?> ObterPerfilAsync(CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<IPerfilService, PerfilDto?>(async s => await s.ObterAsync(ct));

        return await _http!.GetFromJsonAsync<PerfilDto>("/api/perfil", ct);
    }

    public async Task<PerfilDto> AtualizarPerfilAsync(PerfilInputDto input, CancellationToken ct = default)
    {
        if (EmProcesso)
            return await UsarServicoAsync<IPerfilService, PerfilDto>(s => s.AtualizarAsync(input, ct), exigeAdmin: true);

        using var req = CriarRequisicao(HttpMethod.Put, "/api/perfil", input);
        using var resposta = await _http!.SendAsync(req, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<PerfilDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Falha ao atualizar perfil na API.");
    }

    // ==========================================
    // UPLOAD DE MÍDIA (WWWROOT/UPLOADS)
    // ==========================================
    public async Task<string> SalvarUploadMidiaAsync(
        Stream arquivoStream,
        string nomeArquivoOriginal,
        string subpasta,
        string? urlAnterior = null,
        CancellationToken ct = default)
    {
        if (!_adminState.EstaAutenticado)
        {
            throw new UnauthorizedAccessException("Operação restrita para administradores autenticados.");
        }

        if (string.IsNullOrWhiteSpace(_webRootPath))
        {
            throw new InvalidOperationException("Diretório wwwroot não configurado para upload local.");
        }

        var extensoesPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".gif", ".webp", ".png", ".jpg", ".jpeg"
        };

        var extensao = Path.GetExtension(nomeArquivoOriginal);
        if (string.IsNullOrWhiteSpace(extensao) || !extensoesPermitidas.Contains(extensao))
        {
            extensao = ".gif";
        }
        else
        {
            extensao = extensao.ToLowerInvariant();
        }

        var pastaDestino = Path.Combine(_webRootPath, "uploads", subpasta);
        Directory.CreateDirectory(pastaDestino);

        var nomeUnico = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extensao}";
        var caminhoCompleto = Path.Combine(pastaDestino, nomeUnico);

        await using (var fs = new FileStream(
            caminhoCompleto,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 64 * 1024,
            useAsync: true))
        {
            await arquivoStream.CopyToAsync(fs, 64 * 1024, ct);
        }

        RemoverArquivoLocalSeExistir(urlAnterior);

        return $"/uploads/{subpasta}/{nomeUnico}";
    }

    public void RemoverArquivoLocalSeExistir(string? urlRelativa)
    {
        if (string.IsNullOrWhiteSpace(_webRootPath) || string.IsNullOrWhiteSpace(urlRelativa))
            return;

        if (!urlRelativa.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            return;

        try
        {
            var caminhoRelativo = urlRelativa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var caminhoCompleto = Path.GetFullPath(Path.Combine(_webRootPath, caminhoRelativo));
            var pastaUploads = Path.GetFullPath(Path.Combine(_webRootPath, "uploads"));

            if (caminhoCompleto.StartsWith(pastaUploads, StringComparison.OrdinalIgnoreCase) && File.Exists(caminhoCompleto))
            {
                File.Delete(caminhoCompleto);
            }
        }
        catch
        {
            // Ignora falha ao remover arquivo antigo
        }
    }
}
