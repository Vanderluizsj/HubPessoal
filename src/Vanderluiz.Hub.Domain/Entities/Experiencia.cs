namespace Vanderluiz.Hub.Domain.Entities;

public class Experiencia : BaseEntity
{
    public string Cargo { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public string? Local { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }

    public bool Atual => DataFim is null;
}

