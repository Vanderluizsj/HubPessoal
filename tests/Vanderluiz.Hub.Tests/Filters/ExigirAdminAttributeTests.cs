using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vanderluiz.Hub.Api.Filters;
using Xunit;

namespace Vanderluiz.Hub.Tests.Filters;

public class ExigirAdminAttributeTests
{
    private static ActionExecutingContext CriarContexto(string? headerChaveRecebida, string chaveConfigurada)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Admin:ChaveAcesso"] = chaveConfigurada
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        var serviceProvider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        if (headerChaveRecebida is not null)
        {
            httpContext.Request.Headers["X-Admin-Key"] = headerChaveRecebida;
        }

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());
    }

    [Fact]
    public async Task OnActionExecutionAsync_ComChaveCorreta_DevePermitirExecucao()
    {
        // Arrange
        var filtro = new ExigirAdminAttribute();
        var contexto = CriarContexto("minha-chave-secreta", "minha-chave-secreta");
        var proximoExecutado = false;

        ActionExecutionDelegate next = () =>
        {
            proximoExecutado = true;
            return Task.FromResult(new ActionExecutedContext(contexto, new List<IFilterMetadata>(), new object()));
        };

        // Act
        await filtro.OnActionExecutionAsync(contexto, next);

        // Assert
        proximoExecutado.Should().BeTrue();
        contexto.Result.Should().BeNull();
    }

    [Fact]
    public async Task OnActionExecutionAsync_SemHeader_DeveRetornar401Unauthorized()
    {
        // Arrange
        var filtro = new ExigirAdminAttribute();
        var contexto = CriarContexto(null, "chave-secreta");
        var proximoExecutado = false;

        ActionExecutionDelegate next = () =>
        {
            proximoExecutado = true;
            return Task.FromResult(new ActionExecutedContext(contexto, new List<IFilterMetadata>(), new object()));
        };

        // Act
        await filtro.OnActionExecutionAsync(contexto, next);

        // Assert
        proximoExecutado.Should().BeFalse();
        contexto.Result.Should().BeOfType<ObjectResult>();
        var resultado = (ObjectResult)contexto.Result!;
        resultado.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task OnActionExecutionAsync_ComChaveIncorreta_DeveRetornar401Unauthorized()
    {
        // Arrange
        var filtro = new ExigirAdminAttribute();
        var contexto = CriarContexto("chave-errada", "chave-secreta");
        var proximoExecutado = false;

        ActionExecutionDelegate next = () =>
        {
            proximoExecutado = true;
            return Task.FromResult(new ActionExecutedContext(contexto, new List<IFilterMetadata>(), new object()));
        };

        // Act
        await filtro.OnActionExecutionAsync(contexto, next);

        // Assert
        proximoExecutado.Should().BeFalse();
        contexto.Result.Should().BeOfType<ObjectResult>();
        var resultado = (ObjectResult)contexto.Result!;
        resultado.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }
}

