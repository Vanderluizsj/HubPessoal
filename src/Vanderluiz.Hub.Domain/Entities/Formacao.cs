using Vanderluiz.Hub.Domain.Enums;

namespace Vanderluiz.Hub.Domain.Entities;

public class Formacao : BaseEntity
{
    public string Curso { get; set; } = string.Empty;
    public string Instituicao { get; set; } = string.Empty;
    public TipoFormacao Tipo { get; set; }
    public string? Descricao { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }

    public bool EmAndamento => DataFim is null;
}

