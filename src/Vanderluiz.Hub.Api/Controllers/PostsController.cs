using Microsoft.AspNetCore.Mvc;
using Vanderluiz.Hub.Api.Filters;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;

    public PostsController(IPostService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista os posts do blog (somente publicados por padrão, ou todos se ?todos=true). (Público)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PostResumoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int? quantidade = null, [FromQuery] bool todos = false, CancellationToken ct = default)
    {
        var posts = todos ? await _service.ListarTodosAsync(ct) : await _service.ListarPublicadosAsync(quantidade, ct);
        return Ok(posts);
    }

    /// <summary>
    /// Obtém um post específico pelo seu slug. (Público)
    /// </summary>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorSlug(string slug, CancellationToken ct = default)
    {
        var post = await _service.ObterPublicadoPorSlugAsync(slug, ct);
        return post is null ? NotFound(new { mensagem = $"Post com slug '{slug}' não encontrado." }) : Ok(post);
    }

    /// <summary>
    /// Obtém um post específico pelo Id. (Público)
    /// </summary>
    [HttpGet("id/{id:int}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken ct = default)
    {
        var post = await _service.ObterPorIdAsync(id, ct);
        return post is null ? NotFound(new { mensagem = $"Post com Id {id} não encontrado." }) : Ok(post);
    }

    /// <summary>
    /// Cadastra um novo artigo no blog. (Requer Admin)
    /// </summary>
    [HttpPost]
    [ExigirAdmin]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Criar([FromBody] PostInputDto input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.Titulo))
            return BadRequest(new { erro = "O título do post é obrigatório." });

        var post = await _service.CriarAsync(input, ct);
        return CreatedAtAction(nameof(ObterPorSlug), new { slug = post.Slug }, post);
    }

    /// <summary>
    /// Atualiza um artigo existente. (Requer Admin)
    /// </summary>
    [HttpPut("{id:int}")]
    [ExigirAdmin]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] PostInputDto input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.Titulo))
            return BadRequest(new { erro = "O título do post é obrigatório." });

        var atualizado = await _service.AtualizarAsync(id, input, ct);
        if (!atualizado) return NotFound(new { mensagem = $"Post com Id {id} não encontrado." });

        return NoContent();
    }

    /// <summary>
    /// Remove um post pelo Id. (Requer Admin)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ExigirAdmin]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct = default)
    {
        var removido = await _service.RemoverAsync(id, ct);
        if (!removido) return NotFound(new { mensagem = $"Post com Id {id} não encontrado." });

        return NoContent();
    }
}
