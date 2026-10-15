using Microsoft.AspNetCore.Mvc;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TecnologiasController : ControllerBase
{
    private readonly ITecnologiaService _service;

    public TecnologiasController(ITecnologiaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todas as tecnologias cadastradas agrupadas com contagem de projetos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TecnologiaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct = default)
    {
        var tecnologias = await _service.ListarAsync(ct);
        return Ok(tecnologias);
    }
}

