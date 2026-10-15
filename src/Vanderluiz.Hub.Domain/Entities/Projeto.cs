using Vanderluiz.Hub.Domain.Enums;

namespace Vanderluiz.Hub.Domain.Entities;

public class Projeto : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;

    /// <summary>Identificador amigável usado na URL. Ex.: gestao-de-equipamentos</summary>
    public string Slug { get; set; } = string.Empty;

    public string Resumo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? UrlRepositorio { get; set; }
    public string? UrlDemo { get; set; }
    public string? ImagemUrl { get; set; }
    public StatusProjeto Status { get; set; } = StatusProjeto.EmAndamento;
    public bool Destaque { get; set; }
    public int Ordem { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }

    public ICollection<Tecnologia> Tecnologias { get; set; } = new List<Tecnologia>();
}

