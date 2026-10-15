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
    /// Obtém uma experiência pelo Id. (Público)
    /// </summary>
    [HttpGet("experiencias/{id:int}")]
    [ProducesResponseType(typeof(ExperienciaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterExperienciaPorId(int id, CancellationToken ct = default)
    {
        var item = await _service.ObterExperienciaPorIdAsync(id, ct);
        return item is null ? NotFound(new { mensagem = $"Experiência com Id {id} não encontrada." }) : Ok(item);
    }

    /// <summary>
    /// Cadastra uma nova experiência profissional. (Requer Admin)
    /// </summary>
    [HttpPost("experiencias")]
    [Vanderluiz.Hub.Api.Filters.ExigirAdmin]
    [ProducesResponseType(typeof(ExperienciaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CriarExperiencia([FromBody] ExperienciaInputDto input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.Cargo) || string.IsNullOrWhiteSpace(input.Empresa))
            return BadRequest(new { erro = "Cargo e Empresa são obrigatórios." });

        var nova = await _service.CriarExperienciaAsync(input, ct);
        return CreatedAtAction(nameof(ObterExperienciaPorId), new { id = nova.Id }, nova);
    }

    /// <summary>
    /// Atualiza uma experiência profissional. (Requer Admin)
    /// </summary>
    [HttpPut("experiencias/{id:int}")]
    [Vanderluiz.Hub.Api.Filters.ExigirAdmin]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarExperiencia(int id, [FromBody] ExperienciaInputDto input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.Cargo) || string.IsNullOrWhiteSpace(input.Empresa))
            return BadRequest(new { erro = "Cargo e Empresa são obrigatórios." });

        var atualizado = await _service.AtualizarExperienciaAsync(id, input, ct);
        if (!atualizado) return NotFound(new { mensagem = $"Experiência com Id {id} não encontrada." });

        return NoContent();
    }

    /// <summary>
    /// Remove uma experiência profissional pelo Id. (Requer Admin)
    /// </summary>
    [HttpDelete("experiencias/{id:int}")]
    [Vanderluiz.Hub.Api.Filters.ExigirAdmin]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverExperiencia(int id, CancellationToken ct = default)
    {
        var removido = await _service.RemoverExperienciaAsync(id, ct);
        if (!removido) return NotFound(new { mensagem = $"Experiência com Id {id} não encontrada." });

        return NoContent();
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

    /// <summary>
    /// Obtém uma formação pelo Id. (Público)
    /// </summary>
    [HttpGet("formacoes/{id:int}")]
    [ProducesResponseType(typeof(FormacaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterFormacaoPorId(int id, CancellationToken ct = default)
    {
        var item = await _service.ObterFormacaoPorIdAsync(id, ct);
        return item is null ? NotFound(new { mensagem = $"Formação com Id {id} não encontrada." }) : Ok(item);
    }

    /// <summary>
    /// Cadastra uma nova formação acadêmica ou curso. (Requer Admin)
    /// </summary>
    [HttpPost("formacoes")]
    [Vanderluiz.Hub.Api.Filters.ExigirAdmin]
    [ProducesResponseType(typeof(FormacaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CriarFormacao([FromBody] FormacaoInputDto input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.Curso) || string.IsNullOrWhiteSpace(input.Instituicao))
            return BadRequest(new { erro = "Curso e Instituição são obrigatórios." });

        var nova = await _service.CriarFormacaoAsync(input, ct);
        return CreatedAtAction(nameof(ObterFormacaoPorId), new { id = nova.Id }, nova);
    }

    /// <summary>
    /// Atualiza uma formação acadêmica. (Requer Admin)
    /// </summary>
    [HttpPut("formacoes/{id:int}")]
    [Vanderluiz.Hub.Api.Filters.ExigirAdmin]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarFormacao(int id, [FromBody] FormacaoInputDto input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.Curso) || string.IsNullOrWhiteSpace(input.Instituicao))
            return BadRequest(new { erro = "Curso e Instituição são obrigatórios." });

        var atualizado = await _service.AtualizarFormacaoAsync(id, input, ct);
        if (!atualizado) return NotFound(new { mensagem = $"Formação com Id {id} não encontrada." });

        return NoContent();
    }

    /// <summary>
    /// Remove uma formação pelo Id. (Requer Admin)
    /// </summary>
    [HttpDelete("formacoes/{id:int}")]
    [Vanderluiz.Hub.Api.Filters.ExigirAdmin]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverFormacao(int id, CancellationToken ct = default)
    {
        var removido = await _service.RemoverFormacaoAsync(id, ct);
        if (!removido) return NotFound(new { mensagem = $"Formação com Id {id} não encontrada." });

        return NoContent();
    }
}

