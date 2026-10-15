using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Mvc.Models;
using Vanderluiz.Hub.Mvc.ViewModels;

namespace Vanderluiz.Hub.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly IProjetoService _projetos;
    private readonly ITecnologiaService _tecnologias;
    private readonly IPostService _posts;
    private readonly ITrajetoriaService _trajetoria;

    public HomeController(
        IProjetoService projetos,
        ITecnologiaService tecnologias,
        IPostService posts,
        ITrajetoriaService trajetoria)
    {
        _projetos = projetos;
        _tecnologias = tecnologias;
        _posts = posts;
        _trajetoria = trajetoria;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var experiencias = await _trajetoria.ListarExperienciasAsync(ct);

        var vm = new HomeViewModel
        {
            ProjetosDestaque = await _projetos.ListarAsync(somenteDestaques: true, ct),
            Tecnologias = await _tecnologias.ListarAsync(ct),
            UltimosPosts = await _posts.ListarPublicadosAsync(3, ct),
            ExperienciaAtual = experiencias.FirstOrDefault(e => e.Atual)
        };
        return View(vm);
    }

    [HttpGet("/sobre")]
    public IActionResult Sobre() => View();

    [HttpGet("/contato")]
    public IActionResult Contato() => View();

    [HttpGet("/erro/{codigo:int?}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? codigo)
    {
        if (codigo == StatusCodes.Status404NotFound)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("NotFound");
        }

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
