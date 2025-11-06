using Moq;
using MT.Application.Dtos;
using MT.Application.Mappers;
using MT.Application.Services;
using MT.Domain.Entities;
using MT.Domain.Interfaces;

namespace MT.Tests.APP;

public class ColaboradorServiceTests
{
    private readonly Mock<IColaboradorRepository> _colaboradorRepositoryMock;
    private readonly ColaboradorService _colaboradorService;

    public ColaboradorServiceTests()
    {
        _colaboradorRepositoryMock = new Mock<IColaboradorRepository>();
        _colaboradorService = new ColaboradorService(_colaboradorRepositoryMock.Object);
    }

    private static ColaboradorEntity BuildColaborador(
        long id = 1,
        string nome = "Felipe",
        string matricula = "123",
        string email = "felipe@email.com")
    {
        return new ColaboradorEntity
        {
            Id = id,
            Nome = nome,
            Matricula = matricula,
            Email = email
        };
    }

    // ========================================
    // READ
    // ========================================

    [Fact(DisplayName = "ObterTodosColaboradoresAsync - Deve retornar colaboradores com sucesso")]
    public async Task ObterTodosColaboradoresAsync_DeveRetornarColaboradores()
    {
        var colaboradores = new List<ColaboradorEntity>
        {
            BuildColaborador(),
            BuildColaborador(2, "User2", "002", "user2@email.com")
        };

        var page = new PageResultModel<IEnumerable<ColaboradorEntity>>
        {
            Data = colaboradores,
            TotalRegistros = 2,
            Deslocamento = 0,
            RegistrosRetornados = 2
        };

        _colaboradorRepositoryMock
            .Setup(r => r.ObterTodosColaboradoresAsync(0, 10))
            .ReturnsAsync(page);

        var result = await _colaboradorService.ObterTodosColaboradoresAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.TotalRegistros);
    }

    [Fact(DisplayName = "ObterTodosColaboradoresAsync - Deve retornar falha se não houver colaboradores")]
    public async Task ObterTodosColaboradoresAsync_DeveRetornarFalha_SeVazio()
    {
        var page = new PageResultModel<IEnumerable<ColaboradorEntity>>
        {
            Data = new List<ColaboradorEntity>(),
            TotalRegistros = 0
        };

        _colaboradorRepositoryMock
            .Setup(r => r.ObterTodosColaboradoresAsync(0, 10))
            .ReturnsAsync(page);

        var result = await _colaboradorService.ObterTodosColaboradoresAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal("Não existe conteudo para colaborador", result.Error);
    }

    [Fact(DisplayName = "ObterColaboradorPorIdAsync - Deve retornar colaborador existente")]
    public async Task ObterColaboradorPorIdAsync_DeveRetornarColaborador()
    {
        var colaborador = BuildColaborador();

        _colaboradorRepositoryMock
            .Setup(r => r.ObterColaboradorPorIdAsync(colaborador.Id))
            .ReturnsAsync(colaborador);

        var result = await _colaboradorService.ObterColaboradorPorIdAsync(colaborador.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(colaborador.Nome, result.Value!.Nome);
    }

    [Fact(DisplayName = "ObterColaboradorPorIdAsync - Deve falhar se colaborador não existir")]
    public async Task ObterColaboradorPorIdAsync_DeveFalhar_ColaboradorNaoExiste()
    {
        _colaboradorRepositoryMock
            .Setup(r => r.ObterColaboradorPorIdAsync(It.IsAny<long>()))
            .ReturnsAsync((ColaboradorEntity?)null);

        var result = await _colaboradorService.ObterColaboradorPorIdAsync(99);

        Assert.False(result.IsSuccess);
        Assert.Equal("Colaborador não encontrado", result.Error);
    }

    // ========================================
    // CREATE
    // ========================================

    [Fact(DisplayName = "AdicionarColaboradorAsync - Deve adicionar novo colaborador com sucesso")]
    public async Task AdicionarColaboradorAsync_DeveAdicionarComSucesso()
    {
        var dto = new ColaboradorDTO("Felipe", "123", "felipe@email.com");
        var entity = dto.ToColaboradorEntity();

        _colaboradorRepositoryMock
            .Setup(r => r.ObterTodosColaboradoresAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new PageResultModel<IEnumerable<ColaboradorEntity>> { Data = new List<ColaboradorEntity>() });

        _colaboradorRepositoryMock
            .Setup(r => r.AdicionarColaboradorAsync(It.IsAny<ColaboradorEntity>()))
            .ReturnsAsync(entity);

        var result = await _colaboradorService.AdicionarColaboradorAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto.Email, result.Value!.Email);
    }

    [Fact(DisplayName = "AdicionarColaboradorAsync - Deve falhar se já existir email ou matrícula")]
    public async Task AdicionarColaboradorAsync_DeveFalhar_SeDuplicado()
    {
        var dto = new ColaboradorDTO("Novo", "001", "email@teste.com");
        var existente = BuildColaborador(1, "Outro", "001", "email@teste.com");

        _colaboradorRepositoryMock
            .Setup(r => r.ObterTodosColaboradoresAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new PageResultModel<IEnumerable<ColaboradorEntity>> { Data = new List<ColaboradorEntity> { existente } });

        var result = await _colaboradorService.AdicionarColaboradorAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Já existe um colaborador com este e-mail ou matrícula", result.Error);
    }

    // ========================================
    // UPDATE
    // ========================================

    [Fact(DisplayName = "EditarColaboradorAsync - Deve editar colaborador com sucesso")]
    public async Task EditarColaboradorAsync_DeveEditarComSucesso()
    {
        var colaborador = BuildColaborador();
        var dto = new ColaboradorDTO("Atualizado", "123", colaborador.Email);
        var atualizado = dto.ToColaboradorEntity();
        atualizado.Id = colaborador.Id;

        _colaboradorRepositoryMock.Setup(r => r.ObterColaboradorPorIdAsync(colaborador.Id)).ReturnsAsync(colaborador);
        _colaboradorRepositoryMock.Setup(r => r.ExisteOutroComMesmoEmailAsync(colaborador.Id, dto.Email)).ReturnsAsync(false);
        _colaboradorRepositoryMock.Setup(r => r.ExisteOutroComMesmoMatriculaAsync(colaborador.Id, dto.Matricula)).ReturnsAsync(false);
        _colaboradorRepositoryMock.Setup(r => r.EditarColaboradorAsync(colaborador.Id, It.IsAny<ColaboradorEntity>())).ReturnsAsync(atualizado);

        var result = await _colaboradorService.EditarColaboradorAsync(colaborador.Id, dto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Atualizado", result.Value!.Nome);
    }

    [Fact(DisplayName = "EditarColaboradorAsync - Deve falhar se colaborador não existe")]
    public async Task EditarColaboradorAsync_DeveFalhar_ColaboradorNaoExiste()
    {
        var dto = new ColaboradorDTO("Novo", "001", "email@teste.com");

        _colaboradorRepositoryMock.Setup(r => r.ObterColaboradorPorIdAsync(It.IsAny<long>())).ReturnsAsync((ColaboradorEntity?)null);

        var result = await _colaboradorService.EditarColaboradorAsync(99, dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Colaborador não encontrado", result.Error);
    }

    [Fact(DisplayName = "EditarColaboradorAsync - Deve falhar se email duplicado")]
    public async Task EditarColaboradorAsync_DeveFalhar_EmailDuplicado()
    {
        var colaborador = BuildColaborador();
        var dto = new ColaboradorDTO("Novo", "123", "duplicado@teste.com");

        _colaboradorRepositoryMock.Setup(r => r.ObterColaboradorPorIdAsync(colaborador.Id)).ReturnsAsync(colaborador);
        _colaboradorRepositoryMock.Setup(r => r.ExisteOutroComMesmoEmailAsync(colaborador.Id, dto.Email)).ReturnsAsync(true);

        var result = await _colaboradorService.EditarColaboradorAsync(colaborador.Id, dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Já existe outro colaborador com este e-mail", result.Error);
    }

    [Fact(DisplayName = "EditarColaboradorAsync - Deve falhar se matrícula duplicada")]
    public async Task EditarColaboradorAsync_DeveFalhar_MatriculaDuplicada()
    {
        var colaborador = BuildColaborador();
        var dto = new ColaboradorDTO("Novo", "999", colaborador.Email);

        _colaboradorRepositoryMock.Setup(r => r.ObterColaboradorPorIdAsync(colaborador.Id)).ReturnsAsync(colaborador);
        _colaboradorRepositoryMock.Setup(r => r.ExisteOutroComMesmoEmailAsync(colaborador.Id, dto.Email)).ReturnsAsync(false);
        _colaboradorRepositoryMock.Setup(r => r.ExisteOutroComMesmoMatriculaAsync(colaborador.Id, dto.Matricula)).ReturnsAsync(true);

        var result = await _colaboradorService.EditarColaboradorAsync(colaborador.Id, dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Já existe outro colaborador com esta matrícula", result.Error);
    }

    // ========================================
    // DELETE
    // ========================================

    [Fact(DisplayName = "DeletarColaboradorAsync - Deve deletar colaborador com sucesso")]
    public async Task DeletarColaboradorAsync_DeveDeletarComSucesso()
    {
        var colaborador = BuildColaborador();

        _colaboradorRepositoryMock.Setup(r => r.ObterColaboradorPorIdAsync(colaborador.Id)).ReturnsAsync(colaborador);
        _colaboradorRepositoryMock.Setup(r => r.DeletarColaboradorAsync(colaborador.Id)).ReturnsAsync(colaborador);

        var result = await _colaboradorService.DeletarColaboradorAsync(colaborador.Id);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact(DisplayName = "DeletarColaboradorAsync - Deve falhar se colaborador não existe")]
    public async Task DeletarColaboradorAsync_DeveFalhar_ColaboradorNaoExiste()
    {
        _colaboradorRepositoryMock.Setup(r => r.ObterColaboradorPorIdAsync(It.IsAny<long>())).ReturnsAsync((ColaboradorEntity?)null);

        var result = await _colaboradorService.DeletarColaboradorAsync(5);

        Assert.False(result.IsSuccess);
        Assert.Equal("Colaborador não encontrado", result.Error);
    }
}
