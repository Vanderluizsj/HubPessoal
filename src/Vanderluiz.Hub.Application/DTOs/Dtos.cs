using Vanderluiz.Hub.Domain.Enums;

namespace Vanderluiz.Hub.Application.DTOs;

public record TecnologiaTagDto(int Id, string Nome, string? IconeClasse);

public record TecnologiaDto(
    int Id,
    string Nome,
    CategoriaTecnologia Categoria,
    NivelTecnologia Nivel,
    string? IconeClasse,
    int QuantidadeProjetos);

public class TecnologiaInputDto
{
    public string Nome { get; set; } = string.Empty;
    public CategoriaTecnologia Categoria { get; set; } = CategoriaTecnologia.Linguagem;
    public NivelTecnologia Nivel { get; set; } = NivelTecnologia.Intermediario;
    public string? IconeClasse { get; set; }
    public int Ordem { get; set; }
}

public record ProjetoResumoDto(
    int Id,
    string Titulo,
    string Slug,
    string Resumo,
    string? ImagemUrl,
    StatusProjeto Status,
    bool Destaque,
    IReadOnlyList<TecnologiaTagDto> Tecnologias);

public record ProjetoDetalheDto(
    int Id,
    string Titulo,
    string Slug,
    string Resumo,
    string Descricao,
    string? UrlRepositorio,
    string? UrlDemo,
    string? ImagemUrl,
    StatusProjeto Status,
    bool Destaque,
    DateOnly DataInicio,
    DateOnly? DataFim,
    IReadOnlyList<TecnologiaTagDto> Tecnologias);

/// <summary>Dados de entrada para criar/atualizar um projeto.</summary>
public class ProjetoInputDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string Resumo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? UrlRepositorio { get; set; }
    public string? UrlDemo { get; set; }
    public string? ImagemUrl { get; set; }
    public StatusProjeto Status { get; set; }
    public bool Destaque { get; set; }
    public int Ordem { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public List<int> TecnologiaIds { get; set; } = new();
}

public record ExperienciaDto(
    int Id,
    string Cargo,
    string Empresa,
    string? Local,
    string Descricao,
    DateOnly DataInicio,
    DateOnly? DataFim,
    bool Atual);

public class ExperienciaInputDto
{
    public string Cargo { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public string? Local { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
}

public record FormacaoDto(
    int Id,
    string Curso,
    string Instituicao,
    TipoFormacao Tipo,
    string? Descricao,
    DateOnly DataInicio,
    DateOnly? DataFim,
    bool EmAndamento);

public class FormacaoInputDto
{
    public string Curso { get; set; } = string.Empty;
    public string Instituicao { get; set; } = string.Empty;
    public TipoFormacao Tipo { get; set; } = TipoFormacao.Graduacao;
    public string? Descricao { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
}

public record PostResumoDto(int Id, string Titulo, string Slug, string Resumo, DateTime? PublicadoEm);

public record PostDto(int Id, string Titulo, string Slug, string Resumo, string Conteudo, DateTime? PublicadoEm);

public class PostInputDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string Resumo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public bool Publicado { get; set; } = true;
}

public record PerfilDto(
    int Id,
    string Nome,
    string Titulo,
    string Bio,
    string? Localizacao,
    string? Email,
    string? GitHub,
    string? LinkedIn,
    string? FotoUrl,
    string? Telefone);

public class PerfilInputDto
{
    public string Nome { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string? Localizacao { get; set; }
    public string? Email { get; set; }
    public string? GitHub { get; set; }
    public string? LinkedIn { get; set; }
    public string? FotoUrl { get; set; }
    public string? Telefone { get; set; }
}
