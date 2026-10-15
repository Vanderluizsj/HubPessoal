using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Domain.Entities;

namespace Vanderluiz.Hub.Application.Mappings;

/// <summary>Mapeamentos manuais Entidade → DTO (sem dependência de AutoMapper).</summary>
public static class MappingExtensions
{
    public static TecnologiaTagDto ToTagDto(this Tecnologia t) => new(t.Id, t.Nome, t.IconeClasse);

    public static TecnologiaDto ToDto(this Tecnologia t) =>
        new(t.Id, t.Nome, t.Categoria, t.Nivel, t.IconeClasse, t.Projetos.Count);

    public static ProjetoResumoDto ToResumoDto(this Projeto p) =>
        new(p.Id, p.Titulo, p.Slug, p.Resumo, p.ImagemUrl, p.Status, p.Destaque,
            p.Tecnologias.OrderBy(t => t.Ordem).ThenBy(t => t.Nome).Select(t => t.ToTagDto()).ToList());

    public static ProjetoDetalheDto ToDetalheDto(this Projeto p) =>
        new(p.Id, p.Titulo, p.Slug, p.Resumo, p.Descricao, p.UrlRepositorio, p.UrlDemo, p.ImagemUrl,
            p.Status, p.Destaque, p.DataInicio, p.DataFim,
            p.Tecnologias.OrderBy(t => t.Ordem).ThenBy(t => t.Nome).Select(t => t.ToTagDto()).ToList());

    public static ExperienciaDto ToDto(this Experiencia e) =>
        new(e.Id, e.Cargo, e.Empresa, e.Local, e.Descricao, e.DataInicio, e.DataFim, e.Atual);

    public static FormacaoDto ToDto(this Formacao f) =>
        new(f.Id, f.Curso, f.Instituicao, f.Tipo, f.Descricao, f.DataInicio, f.DataFim, f.EmAndamento);

    public static PostResumoDto ToResumoDto(this Post p) => new(p.Id, p.Titulo, p.Slug, p.Resumo, p.PublicadoEm);

    public static PostDto ToDto(this Post p) => new(p.Id, p.Titulo, p.Slug, p.Resumo, p.Conteudo, p.PublicadoEm);

    public static PerfilDto ToDto(this PerfilConfig p) =>
        new(p.Id, p.Nome, p.Titulo, p.Bio, p.Localizacao, p.Email, p.GitHub, p.LinkedIn, p.FotoUrl, p.Telefone);
}

