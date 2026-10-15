using Microsoft.AspNetCore.Mvc;
using Vanderluiz.Hub.Api.Filters;
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
    /// Lista todas as tecnologias cadastradas agrupadas com contagem de projetos. (Público)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TecnologiaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct = default)
    {
        var tecnologias = await _service.ListarAsync(ct);
        return Ok(tecnologias);
    }

    /// <summary>
    /// Obtém os dados de uma tecnologia pelo Id. (Público)
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TecnologiaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken ct = default)
    {
        var tec = await _service.ObterPorIdAsync(id, ct);
        return tec is null ? NotFound(new { mensagem = $"Tecnologia com Id {id} não encontrada." }) : Ok(tec);
    }

    /// <summary>
    /// Cadastra uma nova tecnologia. (Requer Admin)
    /// </summary>
    [HttpPost]
    [ExigirAdmin]
    [ProducesResponseType(typeof(TecnologiaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Criar([FromBody] TecnologiaInputDto input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.Nome))
            return BadRequest(new { erro = "O nome da tecnologia é obrigatório." });

        var nova = await _service.CriarAsync(input, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = nova.Id }, nova);
    }

    /// <summary>
    /// Atualiza uma tecnologia existente. (Requer Admin)
    /// </summary>
    [HttpPut("{id:int}")]
    [ExigirAdmin]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] TecnologiaInputDto input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.Nome))
            return BadRequest(new { erro = "O nome da tecnologia é obrigatório." });

        var atualizado = await _service.AtualizarAsync(id, input, ct);
        if (!atualizado) return NotFound(new { mensagem = $"Tecnologia com Id {id} não encontrada." });

        return NoContent();
    }

    /// <summary>
    /// Remove uma tecnologia pelo Id. (Requer Admin)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ExigirAdmin]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct = default)
    {
        var removido = await _service.RemoverAsync(id, ct);
        if (!removido) return NotFound(new { mensagem = $"Tecnologia com Id {id} não encontrada." });

        return NoContent();
    }
}
