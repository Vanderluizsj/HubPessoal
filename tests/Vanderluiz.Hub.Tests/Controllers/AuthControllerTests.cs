using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Vanderluiz.Hub.Api.Controllers;
using Xunit;

namespace Vanderluiz.Hub.Tests.Controllers;

public class AuthControllerTests
{
    private static IConfiguration CriarConfiguracao(string chaveAdmin)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Admin:ChaveAcesso"] = chaveAdmin
            })
            .Build();
    }

    [Fact]
    public void ValidarChave_ComChaveCorreta_DeveRetornar200Ok()
    {
        // Arrange
        var config = CriarConfiguracao("chave-super-secreta");
        var controller = new AuthController(config);
        var request = new LoginRequest("chave-super-secreta");

        // Act
        var resposta = controller.ValidarChave(request);

        // Assert
        resposta.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)resposta;
        ok.StatusCode.Should().Be(200);
    }

    [Theory]
    [InlineData("chave-errada")]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidarChave_ComChaveIncorretaOuVazia_DeveRetornar401Unauthorized(string chaveInvalida)
    {
        // Arrange
        var config = CriarConfiguracao("chave-super-secreta");
        var controller = new AuthController(config);
        var request = new LoginRequest(chaveInvalida);

        // Act
        var resposta = controller.ValidarChave(request);

        // Assert
        resposta.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorized = (UnauthorizedObjectResult)resposta;
        unauthorized.StatusCode.Should().Be(401);
    }
}

