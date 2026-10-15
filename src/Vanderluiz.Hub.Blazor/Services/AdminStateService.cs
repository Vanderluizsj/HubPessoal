namespace Vanderluiz.Hub.Blazor.Services;

/// <summary>
/// Controla o estado de autenticação do painel administrativo.
/// Permite alternar entre "Modo Leitura (Portfólio)" e "Modo Administrador".
/// </summary>
public class AdminStateService
{
    private const string ChavePadrao = "hub-admin-2026";

    public bool EstaAutenticado { get; private set; }
    public string? ChaveAdmin { get; private set; }

    public event Action? OnChange;

    public bool Autenticar(string senha)
    {
        // Chave padrão ou configurada para o portfólio
        if (senha.Trim() == ChavePadrao)
        {
            EstaAutenticado = true;
            ChaveAdmin = senha.Trim();
            NotificarMudanca();
            return true;
        }

        return false;
    }

    public void Sair()
    {
        EstaAutenticado = false;
        ChaveAdmin = null;
        NotificarMudanca();
    }

    private void NotificarMudanca() => OnChange?.Invoke();
}

