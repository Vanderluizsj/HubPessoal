using Microsoft.AspNetCore.Mvc;
using Vanderluiz.Hub.Api.Filters;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PerfilController : ControllerBase
{
    private readonly IPerfilService _service;

    public PerfilController(IPerfilService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtém as informações de perfil e contato do desenvolvedor. (Público)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PerfilDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Obter(CancellationToken ct = default)
    {
        var perfil = await _service.ObterAsync(ct);
        return Ok(perfil);
    }

    /// <summary>
    /// Atualiza as informações de perfil e contato do desenvolvedor. (Requer Admin)
    /// </summary>
    [HttpPut]
    [ExigirAdmin]
    [ProducesResponseType(typeof(PerfilDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Atualizar([FromBody] PerfilInputDto input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.Nome) || string.IsNullOrWhiteSpace(input.Titulo))
            return BadRequest(new { erro = "Nome e Título são obrigatórios." });

        var atualizado = await _service.AtualizarAsync(input, ct);
        return Ok(atualizado);
    }
}

