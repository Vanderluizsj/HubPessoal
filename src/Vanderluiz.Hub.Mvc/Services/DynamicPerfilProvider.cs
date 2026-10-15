using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Mvc.Services;

/// <summary>
/// Provedor scoped que busca as informações dinâmicas de Perfil/Contato na Web API (ou banco)
/// com fallback para o PerfilOptions configurado estaticamente.
/// </summary>
public class DynamicPerfilProvider
{
    private readonly IPerfilService _perfilService;
    private PerfilDto? _cached;

    public DynamicPerfilProvider(IPerfilService perfilService)
    {
        _perfilService = perfilService;
    }

    public async Task<PerfilDto> GetPerfilAsync(CancellationToken ct = default)
    {
        if (_cached is not null) return _cached;
        try
        {
            _cached = await _perfilService.ObterAsync(ct);
        }
        catch
        {
            _cached = new PerfilDto(
                0,
                "Vanderluiz",
                "Desenvolvedor .NET",
                "Desenvolvedor focado em ASP.NET Core, SQL Server e arquitetura de software.",
                "Brasil",
                "seu-email@exemplo.com",
                "https://github.com/seu-usuario",
                "https://www.linkedin.com/in/seu-usuario",
                null,
                null
            );
        }

        return _cached;
    }
}

