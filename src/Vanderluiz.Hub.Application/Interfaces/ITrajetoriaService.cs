using Vanderluiz.Hub.Application.DTOs;

namespace Vanderluiz.Hub.Application.Interfaces;

/// <summary>Experiências profissionais e formação acadêmica (página /trajetoria).</summary>
public interface ITrajetoriaService
{
    Task<IReadOnlyList<ExperienciaDto>> ListarExperienciasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<FormacaoDto>> ListarFormacoesAsync(CancellationToken ct = default);
}

