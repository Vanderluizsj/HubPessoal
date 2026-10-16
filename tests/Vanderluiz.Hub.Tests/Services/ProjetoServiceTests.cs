using FluentAssertions;
using Moq;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Application.Services;
using Vanderluiz.Hub.Domain.Entities;
using Vanderluiz.Hub.Domain.Enums;
using Xunit;

namespace Vanderluiz.Hub.Tests.Services;

public class ProjetoServiceTests
{
    private readonly Mock<IProjetoRepository> _projetoRepoMock;
    private readonly Mock<ITecnologiaRepository> _tecnologiaRepoMock;
    private readonly ProjetoService _service;

    public ProjetoServiceTests()
    {
        _projetoRepoMock = new Mock<IProjetoRepository>();
        _tecnologiaRepoMock = new Mock<ITecnologiaRepository>();
        _service = new ProjetoService(_projetoRepoMock.Object, _tecnologiaRepoMock.Object);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarProjetosMapeados()
    {
        // Arrange
        var projetos = new List<Projeto>
        {
            new() { Id = 1, Titulo = "Hub Pessoal", Slug = "hub-pessoal", Resumo = "Resumo 1", Status = StatusProjeto.Concluido, Destaque = true },
            new() { Id = 2, Titulo = "API REST", Slug = "api-rest", Resumo = "Resumo 2", Status = StatusProjeto.EmAndamento, Destaque = false }
        };

        _projetoRepoMock.Setup(r => r.ListarComTecnologiasAsync(false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projetos);

        // Act
        var resultado = await _service.ListarAsync(false);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado[0].Titulo.Should().Be("Hub Pessoal");
        resultado[0].Slug.Should().Be("hub-pessoal");
        resultado[0].Destaque.Should().BeTrue();
    }

    [Fact]
    public async Task CriarAsync_QuandoSlugNaoFornecido_DeveGerarSlugAutomatico()
    {
        // Arrange
        var input = new ProjetoInputDto
        {
            Titulo = "Novo Projeto Incrível",
            Slug = null, // Sem slug manual
            Resumo = "Resumo do projeto",
            Descricao = "Descrição detalhada",
            Status = StatusProjeto.EmAndamento,
            DataInicio = new DateOnly(2026, 1, 1),
            TecnologiaIds = [1]
        };

        var tec = new Tecnologia { Id = 1, Nome = ".NET 10" };

        _projetoRepoMock.Setup(r => r.SlugExisteAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _tecnologiaRepoMock.Setup(r => r.ListarPorIdsAsync(input.TecnologiaIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync([tec]);

        _projetoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Projeto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _projetoRepoMock.Setup(r => r.SalvarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var criado = await _service.CriarAsync(input);

        // Assert
        criado.Should().NotBeNull();
        criado.Titulo.Should().Be("Novo Projeto Incrível");
        criado.Slug.Should().Be("novo-projeto-incrivel");
        criado.Tecnologias.Should().ContainSingle(t => t.Nome == ".NET 10");

        _projetoRepoMock.Verify(r => r.AdicionarAsync(It.Is<Projeto>(p => p.Slug == "novo-projeto-incrivel"), It.IsAny<CancellationToken>()), Times.Once);
        _projetoRepoMock.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObterPorSlugAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        // Arrange
        _projetoRepoMock.Setup(r => r.ObterPorSlugAsync("slug-inexistente", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Projeto?)null);

        // Act
        var resultado = await _service.ObterPorSlugAsync("slug-inexistente");

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task RemoverAsync_QuandoProjetoExiste_DeveRemoverERetornarTrue()
    {
        // Arrange
        var projeto = new Projeto { Id = 10, Titulo = "Para Remover" };
        _projetoRepoMock.Setup(r => r.ObterPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projeto);

        _projetoRepoMock.Setup(r => r.SalvarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var sucesso = await _service.RemoverAsync(10);

        // Assert
        sucesso.Should().BeTrue();
        _projetoRepoMock.Verify(r => r.Remover(projeto), Times.Once);
        _projetoRepoMock.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_QuandoProjetoNaoExiste_DeveRetornarFalse()
    {
        // Arrange
        _projetoRepoMock.Setup(r => r.ObterPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Projeto?)null);

        // Act
        var sucesso = await _service.RemoverAsync(999);

        // Assert
        sucesso.Should().BeFalse();
        _projetoRepoMock.Verify(r => r.Remover(It.IsAny<Projeto>()), Times.Never);
    }
}

