using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Application.Mappings;
using Vanderluiz.Hub.Domain.Entities;

namespace Vanderluiz.Hub.Application.Services;

public class PerfilService : IPerfilService
{
    private readonly IRepository<PerfilConfig> _repository;

    public PerfilService(IRepository<PerfilConfig> repository)
    {
        _repository = repository;
    }

    public async Task<PerfilDto> ObterAsync(CancellationToken ct = default)
    {
        var perfil = (await _repository.ListarAsync(ct)).FirstOrDefault();
        if (perfil is null)
        {
            perfil = new PerfilConfig
            {
                Nome = "Vanderluiz",
                Titulo = "Desenvolvedor .NET",
                Bio = "Desenvolvedor focado em ASP.NET Core, SQL Server e arquitetura de software. Construo aplicações web robustas e bem estruturadas.",
                Localizacao = "Brasil",
                Email = "seu-email@exemplo.com",
                GitHub = "https://github.com/seu-usuario",
                LinkedIn = "https://www.linkedin.com/in/seu-usuario"
            };
            await _repository.AdicionarAsync(perfil, ct);
            await _repository.SalvarAsync(ct);
        }

        return perfil.ToDto();
    }

    public async Task<PerfilDto> AtualizarAsync(PerfilInputDto input, CancellationToken ct = default)
    {
        var perfil = (await _repository.ListarAsync(ct)).FirstOrDefault();
        if (perfil is null)
        {
            perfil = new PerfilConfig();
            await _repository.AdicionarAsync(perfil, ct);
        }

        perfil.Nome = input.Nome.Trim();
        perfil.Titulo = input.Titulo.Trim();
        perfil.Bio = input.Bio.Trim();
        perfil.Localizacao = string.IsNullOrWhiteSpace(input.Localizacao) ? null : input.Localizacao.Trim();
        perfil.Email = string.IsNullOrWhiteSpace(input.Email) ? null : input.Email.Trim();
        perfil.GitHub = string.IsNullOrWhiteSpace(input.GitHub) ? null : input.GitHub.Trim();
        perfil.LinkedIn = string.IsNullOrWhiteSpace(input.LinkedIn) ? null : input.LinkedIn.Trim();
        perfil.FotoUrl = string.IsNullOrWhiteSpace(input.FotoUrl) ? null : input.FotoUrl.Trim();
        perfil.Telefone = string.IsNullOrWhiteSpace(input.Telefone) ? null : input.Telefone.Trim();
        perfil.AtualizadoEm = DateTime.UtcNow;

        _repository.Atualizar(perfil);
        await _repository.SalvarAsync(ct);

        return perfil.ToDto();
    }
}

