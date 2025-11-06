using Moq;
using MT.Application.Dtos;
using MT.Application.Mappers;
using MT.Application.Services;
using MT.Domain.Entities;
using MT.Domain.Interfaces;

namespace MT.Tests.APP;

public class PecaServiceTests
{
    private readonly Mock<IPecaRepository> _pecaRepositoryMock;
    private readonly PecaService _pecaService;

    public PecaServiceTests()
    {
        _pecaRepositoryMock = new Mock<IPecaRepository>();
        _pecaService = new PecaService(_pecaRepositoryMock.Object);
    }

    private static PecaEntity BuildPeca(long id = 1, string nome = "Parafuso", string descricao = "Peça de aço", string codigo = "P001", int quantidade = 50)
    {
        return new PecaEntity
        {
            Id = id,
            Nome = nome,
            Descricao = descricao,
            Codigo = codigo,
            QuantidadeEstoque = quantidade
        };
    }

    // ========================================
    // READ
    // ========================================

    [Fact(DisplayName = "ObterTodasPecasAsync - Deve retornar lista de peças com sucesso")]
    public async Task ObterTodasPecasAsync_DeveRetornarPecas()
    {
        var pecas = new List<PecaEntity> { BuildPeca(), BuildPeca(2, "Porca", "Peça roscada", "P002", 100) };

        var page = new PageResultModel<IEnumerable<PecaEntity>>
        {
            Data = pecas,
            TotalRegistros = 2,
            Deslocamento = 0,
            RegistrosRetornados = 2
        };

        _pecaRepositoryMock
            .Setup(r => r.ObterTodasPecasAsync(0, 10))
            .ReturnsAsync(page);

        var result = await _pecaService.ObterTodasPecasAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.TotalRegistros);
    }

    [Fact(DisplayName = "ObterTodasPecasAsync - Deve retornar falha se não houver conteúdo")]
    public async Task ObterTodasPecasAsync_DeveFalhar_SemConteudo()
    {
        var page = new PageResultModel<IEnumerable<PecaEntity>>
        {
            Data = new List<PecaEntity>(),
            TotalRegistros = 0
        };

        _pecaRepositoryMock
            .Setup(r => r.ObterTodasPecasAsync(0, 10))
            .ReturnsAsync(page);

        var result = await _pecaService.ObterTodasPecasAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal("Não existe conteudo para peças", result.Error);
    }

    [Fact(DisplayName = "ObterPecaPorIdAsync - Deve retornar peça existente")]
    public async Task ObterPecaPorIdAsync_DeveRetornarPeca()
    {
        var peca = BuildPeca();

        _pecaRepositoryMock
            .Setup(r => r.ObterPecaPorIdAsync(peca.Id))
            .ReturnsAsync(peca);

        var result = await _pecaService.ObterPecaPorIdAsync(peca.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Parafuso", result.Value!.Nome);
    }

    [Fact(DisplayName = "ObterPecaPorIdAsync - Deve falhar se peça não existir")]
    public async Task ObterPecaPorIdAsync_DeveFalhar_QuandoNaoExiste()
    {
        _pecaRepositoryMock
            .Setup(r => r.ObterPecaPorIdAsync(It.IsAny<long>()))
            .ReturnsAsync((PecaEntity?)null);

        var result = await _pecaService.ObterPecaPorIdAsync(999);

        Assert.False(result.IsSuccess);
        Assert.Equal("Peça não encontrada", result.Error);
    }

    // ========================================
    // CREATE
    // ========================================

    [Fact(DisplayName = "AdicionarPecaAsync - Deve adicionar nova peça com sucesso")]
    public async Task AdicionarPecaAsync_DeveAdicionarPeca()
    {
        var dto = new PecaDTO("Arruela", "Peça pequena", "P003", 25);
        var entity = dto.ToPecaEntity();

        _pecaRepositoryMock
            .Setup(r => r.AdicionarPecaAsync(It.IsAny<PecaEntity>()))
            .ReturnsAsync(entity);

        var result = await _pecaService.AdicionarPecaAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Arruela", result.Value!.Nome);
    }

    [Fact(DisplayName = "AdicionarPecaAsync - Deve retornar falha em caso de exceção")]
    public async Task AdicionarPecaAsync_DeveFalhar_Excecao()
    {
        var dto = new PecaDTO("Erro", "Falha", "P999", 0);

        _pecaRepositoryMock
            .Setup(r => r.AdicionarPecaAsync(It.IsAny<PecaEntity>()))
            .ThrowsAsync(new Exception("Erro no banco"));

        var result = await _pecaService.AdicionarPecaAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Ocorreu um erro ao salvar a peça", result.Error);
    }

    // ========================================
    // UPDATE
    // ========================================

    [Fact(DisplayName = "EditarPecaAsync - Deve editar peça existente com sucesso")]
    public async Task EditarPecaAsync_DeveEditarPeca()
    {
        var peca = BuildPeca();
        var dto = new PecaDTO("Parafuso Atualizado", "Peça revisada", "P001A", 80);
        var atualizada = dto.ToPecaEntity();
        atualizada.Id = peca.Id;

        _pecaRepositoryMock.Setup(r => r.ObterPecaPorIdAsync(peca.Id)).ReturnsAsync(peca);
        _pecaRepositoryMock.Setup(r => r.EditarPecaAsync(peca.Id, It.IsAny<PecaEntity>())).ReturnsAsync(atualizada);

        var result = await _pecaService.EditarPecaAsync(peca.Id, dto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Parafuso Atualizado", result.Value!.Nome);
    }

    [Fact(DisplayName = "EditarPecaAsync - Deve falhar se peça não existir")]
    public async Task EditarPecaAsync_DeveFalhar_QuandoNaoExiste()
    {
        var dto = new PecaDTO("Nova", "Inexistente", "P404", 10);

        _pecaRepositoryMock.Setup(r => r.ObterPecaPorIdAsync(It.IsAny<long>())).ReturnsAsync((PecaEntity?)null);

        var result = await _pecaService.EditarPecaAsync(999, dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Peça não encontrada", result.Error);
    }

    [Fact(DisplayName = "EditarPecaAsync - Deve retornar falha em caso de exceção")]
    public async Task EditarPecaAsync_DeveFalhar_Excecao()
    {
        var peca = BuildPeca();
        var dto = new PecaDTO("Erro", "Teste", "P999", 5);

        _pecaRepositoryMock.Setup(r => r.ObterPecaPorIdAsync(peca.Id)).ReturnsAsync(peca);
        _pecaRepositoryMock.Setup(r => r.EditarPecaAsync(peca.Id, It.IsAny<PecaEntity>())).ThrowsAsync(new Exception("Falha DB"));

        var result = await _pecaService.EditarPecaAsync(peca.Id, dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Ocorreu um erro ao editar a peça", result.Error);
    }

    // ========================================
    // DELETE
    // ========================================

    [Fact(DisplayName = "DeletarPecaAsync - Deve deletar peça com sucesso")]
    public async Task DeletarPecaAsync_DeveDeletarPeca()
    {
        var peca = BuildPeca();

        _pecaRepositoryMock.Setup(r => r.ObterPecaPorIdAsync(peca.Id)).ReturnsAsync(peca);
        _pecaRepositoryMock.Setup(r => r.DeletarPecaAsync(peca.Id)).ReturnsAsync(peca);

        var result = await _pecaService.DeletarPecaAsync(peca.Id);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact(DisplayName = "DeletarPecaAsync - Deve falhar se peça não existir")]
    public async Task DeletarPecaAsync_DeveFalhar_QuandoNaoExiste()
    {
        _pecaRepositoryMock.Setup(r => r.ObterPecaPorIdAsync(It.IsAny<long>())).ReturnsAsync((PecaEntity?)null);

        var result = await _pecaService.DeletarPecaAsync(99);

        Assert.False(result.IsSuccess);
        Assert.Equal("Peça não encontrada", result.Error);
    }

    [Fact(DisplayName = "DeletarPecaAsync - Deve retornar falha em caso de exceção")]
    public async Task DeletarPecaAsync_DeveFalhar_Excecao()
    {
        var peca = BuildPeca();

        _pecaRepositoryMock.Setup(r => r.ObterPecaPorIdAsync(peca.Id)).ReturnsAsync(peca);
        _pecaRepositoryMock.Setup(r => r.DeletarPecaAsync(peca.Id)).ThrowsAsync(new Exception("Falha no banco"));

        var result = await _pecaService.DeletarPecaAsync(peca.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal("Ocorreu um erro ao deletar a peça", result.Error);
    }
}
