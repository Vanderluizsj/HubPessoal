using Microsoft.AspNetCore.Mvc;

namespace Vanderluiz.Hub.Api.Controllers;

public record LoginRequest(string Chave);

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Valida a chave de acesso do administrador.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ValidarChave([FromBody] LoginRequest request)
    {
        var chaveCorreta = _config["Admin:ChaveAcesso"];

        if (string.IsNullOrWhiteSpace(chaveCorreta))
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { erro = "Chave de administrador não configurada no servidor." });
        }

        if (string.IsNullOrWhiteSpace(request.Chave) || request.Chave != chaveCorreta)
        {
            return Unauthorized(new { erro = "Chave de acesso inválida." });
        }

        return Ok(new { sucesso = true, mensagem = "Autenticação realizada com sucesso." });
    }
}

