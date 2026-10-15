using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Domain.Enums;

namespace Vanderluiz.Hub.Mvc.ViewModels;

public class HomeViewModel
{
    public IReadOnlyList<ProjetoResumoDto> ProjetosDestaque { get; init; } = [];
    public IReadOnlyList<TecnologiaDto> Tecnologias { get; init; } = [];
    public IReadOnlyList<PostResumoDto> UltimosPosts { get; init; } = [];
    public ExperienciaDto? ExperienciaAtual { get; init; }
}

public class TrajetoriaViewModel
{
    public IReadOnlyList<ExperienciaDto> Experiencias { get; init; } = [];
    public IReadOnlyList<FormacaoDto> Formacoes { get; init; } = [];
}

public class TecnologiasViewModel
{
    public IReadOnlyList<IGrouping<CategoriaTecnologia, TecnologiaDto>> PorCategoria { get; init; } = [];
}

