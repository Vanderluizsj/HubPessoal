using FluentAssertions;
using Moq;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Application.Services;
using Vanderluiz.Hub.Domain.Entities;
using Vanderluiz.Hub.Domain.Enums;
using Xunit;

namespace Vanderluiz.Hub.Tests.Services;

public class TrajetoriaServiceTests
{
    private readonly Mock<IRepository<Experiencia>> _experienciaRepoMock;
    private readonly Mock<IRepository<Formacao>> _formacaoRepoMock;
    private readonly TrajetoriaService _service;

    public TrajetoriaServiceTests()
    {
        _experienciaRepoMock = new Mock<IRepository<Experiencia>>();
        _formacaoRepoMock = new Mock<IRepository<Formacao>>();
        _service = new TrajetoriaService(_experienciaRepoMock.Object, _formacaoRepoMock.Object);
    }

    [Fact]
    public async Task ListarExperienciasAsync_DeveOrdenarEmpregoAtualPrimeiro()
    {
        // Arrange
        var anterior = new Experiencia
        {
            Id = 1,
            Cargo = "Dev Jr",
            Empresa = "Empresa Antiga",
            DataInicio = new DateOnly(2021, 1, 1),
            DataFim = new DateOnly(2022, 12, 31)
        };

        var atual = new Experiencia
        {
            Id = 2,
            Cargo = "Dev Pleno",
            Empresa = "Empresa Atual",
            DataInicio = new DateOnly(2023, 1, 1),
            DataFim = null // Atual
        };

        _experienciaRepoMock.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([anterior, atual]);

        // Act
        var lista = await _service.ListarExperienciasAsync();

        // Assert
        lista.Should().HaveCount(2);
        lista[0].Id.Should().Be(2); // Emprego atual deve vir primeiro
        lista[0].Atual.Should().BeTrue();
        lista[1].Id.Should().Be(1);
        lista[1].Atual.Should().BeFalse();
    }

    [Fact]
    public async Task CriarExperienciaAsync_DevePreencherDadosESalvar()
    {
        // Arrange
        var input = new ExperienciaInputDto
        {
            Cargo = "Engenheiro de Software",
            Empresa = "Tech Corp",
            Local = "Remoto",
            Descricao = "Construção de APIs e microsserviços",
            DataInicio = new DateOnly(2026, 1, 1),
            DataFim = null
        };

        _experienciaRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Experiencia>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _experienciaRepoMock.Setup(r => r.SalvarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.CriarExperienciaAsync(input);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Cargo.Should().Be("Engenheiro de Software");
        resultado.Empresa.Should().Be("Tech Corp");
        resultado.Atual.Should().BeTrue();

        _experienciaRepoMock.Verify(r => r.AdicionarAsync(It.Is<Experiencia>(e => e.Cargo == "Engenheiro de Software"), It.IsAny<CancellationToken>()), Times.Once);
        _experienciaRepoMock.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarFormacaoAsync_DeveCadastrarESalvar()
    {
        // Arrange
        var input = new FormacaoInputDto
        {
            Curso = "Engenharia de Software",
            Instituicao = "Universidade",
            Tipo = TipoFormacao.Graduacao,
            Descricao = "Foco em sistemas distribuídos",
            DataInicio = new DateOnly(2020, 2, 1),
            DataFim = new DateOnly(2024, 12, 31)
        };

        _formacaoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Formacao>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _formacaoRepoMock.Setup(r => r.SalvarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.CriarFormacaoAsync(input);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Curso.Should().Be("Engenharia de Software");
        resultado.EmAndamento.Should().BeFalse();

        _formacaoRepoMock.Verify(r => r.AdicionarAsync(It.Is<Formacao>(f => f.Curso == "Engenharia de Software"), It.IsAny<CancellationToken>()), Times.Once);
        _formacaoRepoMock.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

