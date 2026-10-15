namespace Vanderluiz.Hub.Domain.Entities;

public class PerfilConfig : BaseEntity
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

