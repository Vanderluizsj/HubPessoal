using Microsoft.AspNetCore.Mvc;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TrajetoriaController : ControllerBase
{
    private readonly ITrajetoriaService _service;

    public TrajetoriaController(ITrajetoriaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista as experiências profissionais da trajetória.
    /// </summary>
    [HttpGet("experiencias")]
    [ProducesResponseType(typeof(IReadOnlyList<ExperienciaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarExperiencias(CancellationToken ct = default)
    {
        var experiencias = await _service.ListarExperienciasAsync(ct);
        return Ok(experiencias);
    }

    /// <summary>
    /// Lista as formações acadêmicas e certificações da trajetória.
    /// </summary>
    [HttpGet("formacoes")]
    [ProducesResponseType(typeof(IReadOnlyList<FormacaoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarFormacoes(CancellationToken ct = default)
    {
        var formacoes = await _service.ListarFormacoesAsync(ct);
        return Ok(formacoes);
    }
}

