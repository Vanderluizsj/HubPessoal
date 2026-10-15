namespace Vanderluiz.Hub.Domain.Entities;

/// <summary>
/// Base para todas as entidades persistidas do Hub.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}

