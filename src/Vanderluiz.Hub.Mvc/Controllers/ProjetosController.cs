using Microsoft.AspNetCore.Mvc;
using Vanderluiz.Hub.Application.Interfaces;

namespace Vanderluiz.Hub.Mvc.Controllers;

[Route("projetos")]
public class ProjetosController : Controller
{
    private readonly IProjetoService _projetos;

    public ProjetosController(IProjetoService projetos) => _projetos = projetos;

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await _projetos.ListarAsync(ct: ct));

    // Ex.: /projetos/gestao-de-equipamentos
    [HttpGet("{slug}")]
    public async Task<IActionResult> Detalhe(string slug, CancellationToken ct)
    {
        var projeto = await _projetos.ObterPorSlugAsync(slug, ct);
        return projeto is null ? NotFound() : View(projeto);
    }
}

