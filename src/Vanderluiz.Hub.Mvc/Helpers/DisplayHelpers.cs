using Vanderluiz.Hub.Domain.Enums;

namespace Vanderluiz.Hub.Mvc.Helpers;

/// <summary>Rótulos amigáveis em pt-BR para enums e períodos.</summary>
public static class DisplayHelpers
{
    public static string Rotulo(this StatusProjeto s) => s switch
    {
        StatusProjeto.Planejado => "Planejado",
        StatusProjeto.EmAndamento => "Em andamento",
        StatusProjeto.Concluido => "Concluído",
        StatusProjeto.Pausado => "Pausado",
        _ => s.ToString()
    };

    public static string CssClasse(this StatusProjeto s) => s switch
    {
        StatusProjeto.Concluido => "status-concluido",
        StatusProjeto.EmAndamento => "status-andamento",
        StatusProjeto.Pausado => "status-pausado",
        _ => "status-planejado"
    };

    public static string Rotulo(this CategoriaTecnologia c) => c switch
    {
        CategoriaTecnologia.Linguagem => "Linguagens",
        CategoriaTecnologia.Backend => "Back-end",
        CategoriaTecnologia.Frontend => "Front-end",
        CategoriaTecnologia.BancoDeDados => "Banco de dados",
        CategoriaTecnologia.DevOps => "DevOps & Cloud",
        CategoriaTecnologia.Ferramenta => "Ferramentas",
        _ => c.ToString()
    };

    public static string Rotulo(this NivelTecnologia n) => n switch
    {
        NivelTecnologia.Basico => "Básico",
        NivelTecnologia.Intermediario => "Intermediário",
        NivelTecnologia.Avancado => "Avançado",
        NivelTecnologia.Especialista => "Especialista",
        _ => n.ToString()
    };

    /// <summary>Percentual para a barra de nível (1..4 → 25..100).</summary>
    public static int Percentual(this NivelTecnologia n) => (int)n * 25;

    public static string Rotulo(this TipoFormacao t) => t switch
    {
        TipoFormacao.Graduacao => "Graduação",
        TipoFormacao.PosGraduacao => "Pós-graduação",
        TipoFormacao.Tecnico => "Técnico",
        TipoFormacao.Curso => "Curso",
        TipoFormacao.Certificacao => "Certificação",
        _ => t.ToString()
    };

    public static string Periodo(DateOnly inicio, DateOnly? fim, string textoAtual = "Atual") =>
        $"{inicio:MMM yyyy} — {(fim.HasValue ? fim.Value.ToString("MMM yyyy") : textoAtual)}";
}

