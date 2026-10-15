using Microsoft.AspNetCore.Mvc;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Mvc.ViewModels;

namespace Vanderluiz.Hub.Mvc.Controllers;

public class TecnologiasController : Controller
{
    private readonly ITecnologiaService _tecnologias;

    public TecnologiasController(ITecnologiaService tecnologias) => _tecnologias = tecnologias;

    [HttpGet("/tecnologias")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var tecnologias = await _tecnologias.ListarAsync(ct);
        var vm = new TecnologiasViewModel
        {
            PorCategoria = tecnologias.GroupBy(t => t.Categoria).OrderBy(g => g.Key).ToList()
        };
        return View(vm);
    }
}

public class TrajetoriaController : Controller
{
    private readonly ITrajetoriaService _trajetoria;

    public TrajetoriaController(ITrajetoriaService trajetoria) => _trajetoria = trajetoria;

    [HttpGet("/trajetoria")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = new TrajetoriaViewModel
        {
            Experiencias = await _trajetoria.ListarExperienciasAsync(ct),
            Formacoes = await _trajetoria.ListarFormacoesAsync(ct)
        };
        return View(vm);
    }
}

[Route("blog")]
public class BlogController : Controller
{
    private readonly IPostService _posts;

    public BlogController(IPostService posts) => _posts = posts;

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await _posts.ListarPublicadosAsync(ct: ct));

    [HttpGet("{slug}")]
    public async Task<IActionResult> Detalhe(string slug, CancellationToken ct)
    {
        var post = await _posts.ObterPublicadoPorSlugAsync(slug, ct);
        return post is null ? NotFound() : View(post);
    }
}

