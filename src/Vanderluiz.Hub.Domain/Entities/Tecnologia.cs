using Vanderluiz.Hub.Domain.Enums;

namespace Vanderluiz.Hub.Domain.Entities;

public class Tecnologia : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public CategoriaTecnologia Categoria { get; set; }
    public NivelTecnologia Nivel { get; set; } = NivelTecnologia.Intermediario;

    /// <summary>Classe CSS do ícone (Devicon). Ex.: devicon-csharp-plain</summary>
    public string? IconeClasse { get; set; }

    public int Ordem { get; set; }

    public ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}

