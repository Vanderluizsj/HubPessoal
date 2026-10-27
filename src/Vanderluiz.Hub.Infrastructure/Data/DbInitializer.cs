using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vanderluiz.Hub.Domain.Entities;
using Vanderluiz.Hub.Domain.Enums;

namespace Vanderluiz.Hub.Infrastructure.Data;

/// <summary>
/// Aplica as migrations pendentes e popula o banco com dados de exemplo quando estiver vazio.
/// Os textos são PLACEHOLDERS — edite aqui (ou futuramente pelo admin em Blazor) com seus dados reais.
/// </summary>
public class DbInitializer
{
    private readonly HubDbContext _db;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(HubDbContext db, ILogger<DbInitializer> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Aplica as migrations pendentes e, em seguida, popula o banco caso esteja vazio.</summary>
    public async Task InicializarAsync(CancellationToken ct = default)
    {
        await MigrarAsync(ct);
        await PopularAsync(ct);
    }

    /// <summary>
    /// Converte qualquer imagem/GIF armazenada como Data URI Base64 no banco para arquivo físico
    /// em wwwroot/uploads/ para evitar tráfego pesado de strings gigantes no SQL Server e no HTML.
    /// </summary>
    public async Task MigrarBase64ParaArquivosAsync(string? webRootPath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(webRootPath))
            return;

        var alterou = false;

        var perfisBase64 = await _db.Perfis
            .Where(p => p.FotoUrl != null && p.FotoUrl.StartsWith("data:image/"))
            .ToListAsync(ct);

        foreach (var perfil in perfisBase64)
        {
            var urlArquivo = SalvarDataUriEmDisco(perfil.FotoUrl!, webRootPath, "perfil");
            if (urlArquivo is not null)
            {
                perfil.FotoUrl = urlArquivo;
                alterou = true;
            }
        }

        var projetosBase64 = await _db.Projetos
            .Where(p => p.ImagemUrl != null && p.ImagemUrl.StartsWith("data:image/"))
            .ToListAsync(ct);

        foreach (var projeto in projetosBase64)
        {
            var urlArquivo = SalvarDataUriEmDisco(projeto.ImagemUrl!, webRootPath, "projetos");
            if (urlArquivo is not null)
            {
                projeto.ImagemUrl = urlArquivo;
                alterou = true;
            }
        }

        if (alterou)
        {
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Mídias em Base64 foram convertidas automaticamente para arquivos físicos em wwwroot/uploads/.");
        }
    }

    private string? SalvarDataUriEmDisco(string dataUri, string webRootPath, string subpasta)
    {
        try
        {
            var indiceVirgula = dataUri.IndexOf(',');
            if (indiceVirgula <= 0) return null;

            var cabecalho = dataUri[..indiceVirgula];
            var base64 = dataUri[(indiceVirgula + 1)..];

            var extensao = ".png";
            if (cabecalho.Contains("image/gif", StringComparison.OrdinalIgnoreCase)) extensao = ".gif";
            else if (cabecalho.Contains("image/webp", StringComparison.OrdinalIgnoreCase)) extensao = ".webp";
            else if (cabecalho.Contains("image/jpeg", StringComparison.OrdinalIgnoreCase) || cabecalho.Contains("image/jpg", StringComparison.OrdinalIgnoreCase)) extensao = ".jpg";

            var bytes = Convert.FromBase64String(base64);
            var pastaDestino = Path.Combine(webRootPath, "uploads", subpasta);
            Directory.CreateDirectory(pastaDestino);

            var nomeUnico = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extensao}";
            var caminhoCompleto = Path.Combine(pastaDestino, nomeUnico);

            File.WriteAllBytes(caminhoCompleto, bytes);
            return $"/uploads/{subpasta}/{nomeUnico}";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível converter mídia Base64 para arquivo em {Subpasta}.", subpasta);
            return null;
        }
    }

    /// <summary>
    /// Aplica automaticamente as migrations pendentes (somente quando o provider é SQL Server,
    /// evitando erro em providers como InMemory usados em testes).
    /// </summary>
    public async Task MigrarAsync(CancellationToken ct = default)
    {
        if (!_db.Database.IsSqlServer())
        {
            _logger.LogInformation("Provider não é SQL Server; migrations ignoradas.");
            return;
        }

        var pendentes = (await _db.Database.GetPendingMigrationsAsync(ct)).ToList();
        if (pendentes.Count == 0)
        {
            _logger.LogInformation("Banco de dados já está atualizado.");
            return;
        }

        _logger.LogInformation("Aplicando {Quantidade} migration(s): {Migrations}", pendentes.Count, string.Join(", ", pendentes));
        await _db.Database.MigrateAsync(ct);
    }

    /// <summary>Insere dados de exemplo somente se o banco estiver vazio.</summary>
    public async Task PopularAsync(CancellationToken ct = default)
    {
        if (await _db.Tecnologias.AnyAsync(ct))
        {
            _logger.LogInformation("Banco já possui dados; seed ignorado.");
            return;
        }

        _logger.LogInformation("Populando banco com dados de exemplo...");

        var csharp = Tec("C#", CategoriaTecnologia.Linguagem, NivelTecnologia.Avancado, "devicon-csharp-plain", 1);
        var dotnet = Tec(".NET", CategoriaTecnologia.Backend, NivelTecnologia.Avancado, "devicon-dotnetcore-plain", 1);
        var aspnet = Tec("ASP.NET Core", CategoriaTecnologia.Backend, NivelTecnologia.Avancado, "devicon-dot-net-plain", 2);
        var efcore = Tec("Entity Framework Core", CategoriaTecnologia.Backend, NivelTecnologia.Intermediario, "devicon-dot-net-plain", 3);
        var blazor = Tec("Blazor", CategoriaTecnologia.Frontend, NivelTecnologia.Basico, "devicon-blazor-original", 1);
        var html = Tec("HTML5", CategoriaTecnologia.Frontend, NivelTecnologia.Avancado, "devicon-html5-plain", 2);
        var css = Tec("CSS3", CategoriaTecnologia.Frontend, NivelTecnologia.Intermediario, "devicon-css3-plain", 3);
        var js = Tec("JavaScript", CategoriaTecnologia.Linguagem, NivelTecnologia.Intermediario, "devicon-javascript-plain", 2);
        var bootstrap = Tec("Bootstrap", CategoriaTecnologia.Frontend, NivelTecnologia.Intermediario, "devicon-bootstrap-plain", 4);
        var sqlserver = Tec("SQL Server", CategoriaTecnologia.BancoDeDados, NivelTecnologia.Avancado, "devicon-microsoftsqlserver-plain", 1);
        var git = Tec("Git", CategoriaTecnologia.Ferramenta, NivelTecnologia.Avancado, "devicon-git-plain", 1);
        var docker = Tec("Docker", CategoriaTecnologia.DevOps, NivelTecnologia.Basico, "devicon-docker-plain", 1);
        var azure = Tec("Azure", CategoriaTecnologia.DevOps, NivelTecnologia.Basico, "devicon-azure-plain", 2);

        _db.Tecnologias.AddRange(csharp, dotnet, aspnet, efcore, blazor, html, css, js, bootstrap, sqlserver, git, docker, azure);

        _db.Projetos.AddRange(
            new Projeto
            {
                Titulo = "Vanderluiz Hub",
                Slug = "vanderluiz-hub",
                Resumo = "Este portfólio: um hub pessoal construído com ASP.NET Core MVC em arquitetura em camadas.",
                Descricao = "Projeto pessoal para centralizar trajetória, projetos e estudos.\n\n" +
                            "Arquitetura: Domain, Application, Infrastructure e MVC, com SQL Server via EF Core. " +
                            "Próximas etapas: extrair uma Web API e criar o painel administrativo em Blazor.",
                UrlRepositorio = "https://github.com/seu-usuario/vanderluiz-hub",
                Status = StatusProjeto.EmAndamento,
                Destaque = true,
                Ordem = 1,
                DataInicio = new DateOnly(2026, 10, 1),
                Tecnologias = { csharp, aspnet, efcore, sqlserver, bootstrap, git }
            },
            new Projeto
            {
                Titulo = "Gestão de Equipamentos",
                Slug = "gestao-de-equipamentos",
                Resumo = "Sistema para controle de inventário, empréstimos e manutenção de equipamentos.",
                Descricao = "Descreva aqui o problema que o sistema resolve, as principais funcionalidades " +
                            "e os desafios técnicos que você enfrentou.",
                Status = StatusProjeto.Concluido,
                Destaque = true,
                Ordem = 2,
                DataInicio = new DateOnly(2025, 3, 1),
                DataFim = new DateOnly(2025, 9, 30),
                Tecnologias = { csharp, aspnet, sqlserver, js, bootstrap }
            },
            new Projeto
            {
                Titulo = "Projeto de Estudo Blazor",
                Slug = "projeto-de-estudo-blazor",
                Resumo = "Laboratório para explorar componentes, estado e consumo de APIs com Blazor.",
                Descricao = "Espaço para experimentos que vão alimentar a Etapa 3 do Hub (painel administrativo).",
                Status = StatusProjeto.Planejado,
                Destaque = true,
                Ordem = 3,
                DataInicio = new DateOnly(2026, 11, 1),
                Tecnologias = { csharp, blazor, docker }
            });

        _db.Experiencias.AddRange(
            new Experiencia
            {
                Cargo = "Desenvolvedor .NET",
                Empresa = "Empresa Atual",
                Local = "Cidade - UF",
                Descricao = "Desenvolvimento e manutenção de sistemas web com ASP.NET Core e SQL Server.\n" +
                            "Integrações com serviços externos e melhoria de performance de consultas.",
                DataInicio = new DateOnly(2023, 1, 1)
            },
            new Experiencia
            {
                Cargo = "Desenvolvedor Júnior",
                Empresa = "Empresa Anterior",
                Local = "Cidade - UF",
                Descricao = "Suporte a sistemas legados, correção de bugs e criação de relatórios.",
                DataInicio = new DateOnly(2021, 2, 1),
                DataFim = new DateOnly(2022, 12, 31)
            });

        _db.Formacoes.AddRange(
            new Formacao
            {
                Curso = "Análise e Desenvolvimento de Sistemas",
                Instituicao = "Nome da Instituição",
                Tipo = TipoFormacao.Graduacao,
                DataInicio = new DateOnly(2019, 2, 1),
                DataFim = new DateOnly(2021, 12, 31)
            },
            new Formacao
            {
                Curso = "Arquitetura de Software",
                Instituicao = "Nome da Instituição",
                Tipo = TipoFormacao.PosGraduacao,
                Descricao = "Clean Architecture, DDD, microsserviços e cloud.",
                DataInicio = new DateOnly(2026, 3, 1)
            });

        _db.Posts.Add(new Post
        {
            Titulo = "Por que criei o meu Hub pessoal",
            Slug = "por-que-criei-o-meu-hub-pessoal",
            Resumo = "A ideia por trás deste projeto e o plano de evolução em etapas.",
            Conteudo = "Este Hub nasceu como um laboratório para praticar arquitetura em camadas.\n\n" +
                       "Etapa 1: MVC + Application + Domain + Infrastructure.\n" +
                       "Etapa 2: Web API consumida pelo MVC.\n" +
                       "Etapa 3: painel administrativo em Blazor.\n" +
                       "Etapa 4: recursos avançados.",
            Publicado = true,
            PublicadoEm = new DateTime(2026, 10, 15, 12, 0, 0, DateTimeKind.Utc)
        });

        _db.Perfis.Add(new PerfilConfig
        {
            Nome = "Vanderluiz",
            Titulo = "Desenvolvedor .NET",
            Bio = "Desenvolvedor focado em ASP.NET Core, SQL Server e arquitetura de software. Construo aplicações web robustas e bem estruturadas.",
            Localizacao = "Brasil",
            Email = "vanderluiz@exemplo.com",
            GitHub = "https://github.com/seu-usuario",
            LinkedIn = "https://www.linkedin.com/in/seu-usuario"
        });

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed concluído.");
    }

    private static Tecnologia Tec(string nome, CategoriaTecnologia cat, NivelTecnologia nivel, string icone, int ordem) =>
        new() { Nome = nome, Categoria = cat, Nivel = nivel, IconeClasse = icone, Ordem = ordem };
}
