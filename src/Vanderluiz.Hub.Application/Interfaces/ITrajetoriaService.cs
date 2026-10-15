using Vanderluiz.Hub.Application.DTOs;

namespace Vanderluiz.Hub.Application.Interfaces;

/// <summary>Experiências profissionais e formação acadêmica (página /trajetoria).</summary>
public interface ITrajetoriaService
{
    Task<IReadOnlyList<ExperienciaDto>> ListarExperienciasAsync(CancellationToken ct = default);
    Task<ExperienciaDto?> ObterExperienciaPorIdAsync(int id, CancellationToken ct = default);
    Task<ExperienciaDto> CriarExperienciaAsync(ExperienciaInputDto input, CancellationToken ct = default);
    Task<bool> AtualizarExperienciaAsync(int id, ExperienciaInputDto input, CancellationToken ct = default);
    Task<bool> RemoverExperienciaAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<FormacaoDto>> ListarFormacoesAsync(CancellationToken ct = default);
    Task<FormacaoDto?> ObterFormacaoPorIdAsync(int id, CancellationToken ct = default);
    Task<FormacaoDto> CriarFormacaoAsync(FormacaoInputDto input, CancellationToken ct = default);
    Task<bool> AtualizarFormacaoAsync(int id, FormacaoInputDto input, CancellationToken ct = default);
    Task<bool> RemoverFormacaoAsync(int id, CancellationToken ct = default);
}

