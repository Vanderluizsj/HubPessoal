using Microsoft.AspNetCore.Mvc;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjetosController : ControllerBase
{
    private readonly IProjetoService _service;

    public ProjetosController(IProjetoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todos os projetos cadastrados, podendo filtrar apenas destaques.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjetoResumoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] bool somenteDestaques = false, CancellationToken ct = default)
    {
        var projetos = await _service.ListarAsync(somenteDestaques, ct);
        return Ok(projetos);
    }

    /// <summary>
    /// Obtém detalhes de um projeto a partir do seu slug (ex: gestao-de-equipamentos).
    /// </summary>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(ProjetoDetalheDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorSlug(string slug, CancellationToken ct = default)
    {
        var projeto = await _service.ObterPorSlugAsync(slug, ct);
        return projeto is null ? NotFound(new { mensagem = $"Projeto com slug '{slug}' não encontrado." }) : Ok(projeto);
    }

    /// <summary>
    /// Obtém detalhes de um projeto a partir do seu identificador numérico.
    /// </summary>
    [HttpGet("id/{id:int}")]
    [ProducesResponseType(typeof(ProjetoDetalheDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken ct = default)
    {
        var projeto = await _service.ObterPorIdAsync(id, ct);
        return projeto is null ? NotFound(new { mensagem = $"Projeto com Id {id} não encontrado." }) : Ok(projeto);
    }

    /// <summary>
    /// Cria um novo projeto.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjetoDetalheDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] ProjetoInputDto input, CancellationToken ct = default)
    {
        try
        {
            var novoProjeto = await _service.CriarAsync(input, ct);
            return CreatedAtAction(nameof(ObterPorSlug), new { slug = novoProjeto.Slug }, novoProjeto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza um projeto existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ProjetoInputDto input, CancellationToken ct = default)
    {
        try
        {
            var atualizado = await _service.AtualizarAsync(id, input, ct);
            if (!atualizado) return NotFound(new { mensagem = $"Projeto com Id {id} não encontrado." });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    /// <summary>
    /// Remove um projeto pelo identificador.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct = default)
    {
        var removido = await _service.RemoverAsync(id, ct);
        if (!removido) return NotFound(new { mensagem = $"Projeto com Id {id} não encontrado." });

        return NoContent();
    }
}

