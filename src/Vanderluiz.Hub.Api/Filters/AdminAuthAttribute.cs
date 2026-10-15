using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Vanderluiz.Hub.Api.Filters;

/// <summary>
/// Filtro que protege endpoints de mutação (POST/PUT/DELETE) exigindo o header 'X-Admin-Key'.
/// Permite que requisições de leitura permaneçam públicas para demonstração do portfólio.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ExigirAdminAttribute : Attribute, IAsyncActionFilter
{
    public const string HeaderName = "X-Admin-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var chaveConfigurada = config["Admin:ChaveAcesso"] ?? "hub-admin-2026";

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var chaveRecebida) ||
            chaveRecebida != chaveConfigurada)
        {
            context.Result = new ObjectResult(new
            {
                erro = "Não autorizado",
                mensagem = "Operação restrita ao Administrador. Forneça uma chave de acesso válida no header 'X-Admin-Key' ou faça login no painel administrativo."
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        await next();
    }
}

