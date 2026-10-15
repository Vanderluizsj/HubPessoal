namespace Vanderluiz.Hub.Mvc.Options;

/// <summary>Dados pessoais exibidos no site (seção "Perfil" do appsettings.json).</summary>
public class PerfilOptions
{
    public const string Secao = "Perfil";

    public string Nome { get; set; } = "Vanderluiz";
    public string Titulo { get; set; } = "Desenvolvedor .NET";
    public string Bio { get; set; } = string.Empty;
    public string? Localizacao { get; set; }
    public string? Email { get; set; }
    public string? GitHub { get; set; }
    public string? LinkedIn { get; set; }
    public string? FotoUrl { get; set; }
}

