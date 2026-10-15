namespace Vanderluiz.Hub.Blazor.Services;

/// <summary>
/// Controla o estado de autenticação do painel administrativo.
/// Permite alternar entre "Modo Leitura (Portfólio)" e "Modo Administrador".
/// </summary>
public class AdminStateService
{
    public bool EstaAutenticado { get; private set; }
    public string? ChaveAdmin { get; private set; }

    public event Action? OnChange;

    public void DefinirAutenticado(string chave)
    {
        EstaAutenticado = true;
        ChaveAdmin = chave.Trim();
        NotificarMudanca();
    }

    public void Sair()
    {
        EstaAutenticado = false;
        ChaveAdmin = null;
        NotificarMudanca();
    }

    private void NotificarMudanca() => OnChange?.Invoke();
}
