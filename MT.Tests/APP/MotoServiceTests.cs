using Moq;
using MT.Application.Services;
using MT.Domain.Entities;
using MT.Domain.Enums;
using MT.Domain.Interfaces;

namespace MT.Tests.APP;

public class MotoServiceTests
{
    private readonly Mock<IMotoRepository> _motoRepositoryMock;
    private readonly MotoService _motoService;

    public MotoServiceTests()
    {
        _motoRepositoryMock = new Mock<IMotoRepository>();
        _motoService = new MotoService(_motoRepositoryMock.Object);
    }

    // ========================================
    // HELPERS
    // ========================================

    private static MotoEntity BuildMoto(
        long id = 1,
        string placa = "ABC1234",
        string chassi = "CHASSI12345678901",
        ModeloMoto modelo = ModeloMoto.MOTTU_POP,
        StatusMoto status = StatusMoto.DISPONIVEL)
    {
        return new MotoEntity
        {
            Id = id,
            Placa = placa,
            Chassi = chassi,
            Modelo = modelo,
            Status = status
        };
    }

    // ========================================
    // READ - ObterTodasMotosAsync
    // ========================================

    [Fact(DisplayName = "ObterTodasMotosAsync - Deve retornar lista de motos com sucesso")]
    public async Task ObterTodasMotosAsync_DeveRetornarMotos()
    {
        // Arrange
        var motos = new List<MotoEntity>
        {
            BuildMoto(1, "AAA1111", "CHASSI00000000001"),
            BuildMoto(2, "BBB2222", "CHASSI00000000002", ModeloMoto.MOTTU_SPORT)
        };

        var page = new PageResultModel<IEnumerable<MotoEntity>>
        {
            Data = motos,
            TotalRegistros = 2,
            Deslocamento = 0,
            RegistrosRetornados = 2
        };

        _motoRepositoryMock
            .Setup(r => r.ObterTodasMotosAsync(0, 10))
            .ReturnsAsync(page);

        // Act
        var result = await _motoService.ObterTodasMotosAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.TotalRegistros);
    }

    [Fact(DisplayName = "ObterTodasMotosAsync - Deve retornar falha se não houver motos")]
    public async Task ObterTodasMotosAsync_DeveRetornarFalha_ListaVazia()
    {
        // Arrange
        var emptyPage = new PageResultModel<IEnumerable<MotoEntity>>
        {
            Data = new List<MotoEntity>(),
            TotalRegistros = 0
        };

        _motoRepositoryMock
            .Setup(r => r.ObterTodasMotosAsync(0, 10))
            .ReturnsAsync(emptyPage);

        // Act
        var result = await _motoService.ObterTodasMotosAsync();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Não existe conteudo para motos", result.Error);
    }

    [Fact(DisplayName = "ObterTodasMotosAsync - Deve retornar falha em caso de exceção")]
    public async Task ObterTodasMotosAsync_DeveRetornarFalha_Excecao()
    {
        // Arrange
        _motoRepositoryMock
            .Setup(r => r.ObterTodasMotosAsync(0, 10))
            .ThrowsAsync(new Exception("Erro no banco"));

        // Act
        var result = await _motoService.ObterTodasMotosAsync();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Ocorreu um erro ao buscar as motos", result.Error);
    }

    // ========================================
    // READ - ObterMotoPorIdAsync
    // ========================================

    [Fact(DisplayName = "ObterMotoPorIdAsync - Deve retornar moto existente")]
    public async Task ObterMotoPorIdAsync_DeveRetornarMoto()
    {
        // Arrange
        var moto = BuildMoto();

        _motoRepositoryMock
            .Setup(r => r.ObterMotoPorIdAsync(moto.Id))
            .ReturnsAsync(moto);

        // Act
        var result = await _motoService.ObterMotoPorIdAsync(moto.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(moto.Placa, result.Value!.Placa);
    }

    [Fact(DisplayName = "ObterMotoPorIdAsync - Deve retornar falha se moto não existir")]
    public async Task ObterMotoPorIdAsync_DeveFalhar_MotoNaoExiste()
    {
        // Arrange
        _motoRepositoryMock
            .Setup(r => r.ObterMotoPorIdAsync(It.IsAny<long>()))
            .ReturnsAsync((MotoEntity?)null);

        // Act
        var result = await _motoService.ObterMotoPorIdAsync(99);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Moto não encontrada", result.Error);
    }

    [Fact(DisplayName = "ObterMotoPorIdAsync - Deve retornar falha em caso de exceção")]
    public async Task ObterMotoPorIdAsync_DeveRetornarFalha_Excecao()
    {
        // Arrange
        _motoRepositoryMock
            .Setup(r => r.ObterMotoPorIdAsync(It.IsAny<long>()))
            .ThrowsAsync(new Exception("Falha inesperada"));

        // Act
        var result = await _motoService.ObterMotoPorIdAsync(1);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Ocorreu um erro ao buscar a moto", result.Error);
    }
}
