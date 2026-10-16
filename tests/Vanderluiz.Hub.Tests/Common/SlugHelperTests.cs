using FluentAssertions;
using Vanderluiz.Hub.Application.Common;
using Xunit;

namespace Vanderluiz.Hub.Tests.Common;

public class SlugHelperTests
{
    [Theory]
    [InlineData("Gestão de Equipamentos", "gestao-de-equipamentos")]
    [InlineData("Meu Portfólio Pessoal em .NET 10!", "meu-portfolio-pessoal-em-net-10")]
    [InlineData("   Espaços no Início e Fim   ", "espacos-no-inicio-e-fim")]
    [InlineData("C# com ASP.NET Core & Blazor", "c-com-asp-net-core-blazor")]
    [InlineData("Acentuação: áéíóú àèìòù ãõ âêîôû ç", "acentuacao-aeiou-aeiou-ao-aeiou-c")]
    public void Gerar_DeveNormalizarTextoERemoverCaracteresEspeciais(string entrada, string esperado)
    {
        // Act
        var resultado = SlugHelper.Gerar(entrada);

        // Assert
        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Gerar_EntradaNulaOuVazia_DeveRetornarStringVazia(string? entrada)
    {
        // Act
        var resultado = SlugHelper.Gerar(entrada!);

        // Assert
        resultado.Should().BeEmpty();
    }
}

