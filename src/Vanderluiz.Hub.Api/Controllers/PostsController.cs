using Microsoft.AspNetCore.Mvc;
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
    /// Lista os posts publicados do blog.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PostResumoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int? quantidade = null, CancellationToken ct = default)
    {
        var posts = await _service.ListarPublicadosAsync(quantidade, ct);
        return Ok(posts);
    }

    /// <summary>
    /// Obtém um post específico pelo seu slug.
    /// </summary>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorSlug(string slug, CancellationToken ct = default)
    {
        var post = await _service.ObterPublicadoPorSlugAsync(slug, ct);
        return post is null ? NotFound(new { mensagem = $"Post com slug '{slug}' não encontrado." }) : Ok(post);
    }
}

