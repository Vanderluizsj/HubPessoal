namespace Vanderluiz.Hub.Domain.Entities;

public class Post : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Resumo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public bool Publicado { get; set; }
    public DateTime? PublicadoEm { get; set; }
}

