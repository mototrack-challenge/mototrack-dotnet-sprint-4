using Moq;
using MT.Application.Dtos;
using MT.Application.Mappers;
using MT.Application.Services;
using MT.Domain.Entities;
using MT.Domain.Interfaces;
using MT.Domain.Enums;

namespace MT.Tests.APP;

public class ServicoServiceTests
{
    private readonly Mock<IServicoRepository> _servicoRepositoryMock;
    private readonly Mock<IMotoRepository> _motoRepositoryMock;
    private readonly ServicoService _servicoService;

    public ServicoServiceTests()
    {
        _servicoRepositoryMock = new Mock<IServicoRepository>();
        _motoRepositoryMock = new Mock<IMotoRepository>();
        _servicoService = new ServicoService(_servicoRepositoryMock.Object, _motoRepositoryMock.Object);
    }

    // ========================================
    // HELPERS
    // ========================================

    private static ServicoEntity BuildServico(long id = 1, long motoId = 10, string descricao = "Troca de óleo")
    {
        return new ServicoEntity
        {
            Id = id,
            MotoId = motoId,
            Descricao = descricao
        };
    }

    private static MotoEntity BuildMoto(long id = 10)
    {
        return new MotoEntity
        {
            Id = id,
            Placa = "ABC1234",
            Chassi = "123456789ABCDEFG",
            Modelo = MT.Domain.Enums.ModeloMoto.MOTTU_POP,
            Status = MT.Domain.Enums.StatusMoto.DISPONIVEL
        };
    }

    // ========================================
    // READ
    // ========================================

    [Fact(DisplayName = "ObterTodosServicosAsync - Deve retornar lista de serviços com sucesso")]
    public async Task ObterTodosServicosAsync_DeveRetornarServicos()
    {
        var servicos = new List<ServicoEntity> { BuildServico(), BuildServico(2, 10, "Revisão geral") };

        var page = new PageResultModel<IEnumerable<ServicoEntity>>
        {
            Data = servicos,
            TotalRegistros = 2,
            Deslocamento = 0,
            RegistrosRetornados = 2
        };

        _servicoRepositoryMock
            .Setup(r => r.ObterTodosServicosAsync(0, 10))
            .ReturnsAsync(page);

        var result = await _servicoService.ObterTodosServicosAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.TotalRegistros);
    }

    [Fact(DisplayName = "ObterTodosServicosAsync - Deve retornar falha se não houver serviços")]
    public async Task ObterTodosServicosAsync_DeveFalhar_SemServicos()
    {
        var page = new PageResultModel<IEnumerable<ServicoEntity>>
        {
            Data = new List<ServicoEntity>(),
            TotalRegistros = 0
        };

        _servicoRepositoryMock
            .Setup(r => r.ObterTodosServicosAsync(0, 10))
            .ReturnsAsync(page);

        var result = await _servicoService.ObterTodosServicosAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal("Não existe conteudo para serviços", result.Error);
    }

    [Fact(DisplayName = "ObterServicoPorIdAsync - Deve retornar serviço existente")]
    public async Task ObterServicoPorIdAsync_DeveRetornarServico()
    {
        var servico = BuildServico();

        _servicoRepositoryMock
            .Setup(r => r.ObterServicoPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        var result = await _servicoService.ObterServicoPorIdAsync(servico.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(servico.Id, result.Value!.Id);
    }

    [Fact(DisplayName = "ObterServicoPorIdAsync - Deve retornar falha se serviço não existe")]
    public async Task ObterServicoPorIdAsync_DeveFalhar_ServicoNaoExiste()
    {
        _servicoRepositoryMock
            .Setup(r => r.ObterServicoPorIdAsync(It.IsAny<long>()))
            .ReturnsAsync((ServicoEntity?)null);

        var result = await _servicoService.ObterServicoPorIdAsync(99);

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado", result.Error);
    }

    [Fact(DisplayName = "ObterServicosPorMotoIdAsync - Deve retornar serviços da moto")]
    public async Task ObterServicosPorMotoIdAsync_DeveRetornarServicosDaMoto()
    {
        var moto = BuildMoto();
        var servicos = new List<ServicoEntity> { BuildServico(1, moto.Id), BuildServico(2, moto.Id, "Troca de pneu") };

        _motoRepositoryMock.Setup(r => r.ObterMotoPorIdAsync(moto.Id)).ReturnsAsync(moto);
        _servicoRepositoryMock.Setup(r => r.ObterServicosPorMotoIdAsync(moto.Id)).ReturnsAsync(servicos);

        var result = await _servicoService.ObterServicosPorMotoIdAsync(moto.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact(DisplayName = "ObterServicosPorMotoIdAsync - Deve falhar se moto não existe")]
    public async Task ObterServicosPorMotoIdAsync_DeveFalhar_MotoNaoExiste()
    {
        _motoRepositoryMock.Setup(r => r.ObterMotoPorIdAsync(It.IsAny<long>())).ReturnsAsync((MotoEntity?)null);

        var result = await _servicoService.ObterServicosPorMotoIdAsync(10);

        Assert.False(result.IsSuccess);
        Assert.Equal("Moto não encontrada", result.Error);
    }

    [Fact(DisplayName = "ObterServicosPorMotoIdAsync - Deve falhar se não existem serviços para a moto")]
    public async Task ObterServicosPorMotoIdAsync_DeveFalhar_SemServicos()
    {
        var moto = BuildMoto();

        _motoRepositoryMock.Setup(r => r.ObterMotoPorIdAsync(moto.Id)).ReturnsAsync(moto);
        _servicoRepositoryMock.Setup(r => r.ObterServicosPorMotoIdAsync(moto.Id)).ReturnsAsync(new List<ServicoEntity>());

        var result = await _servicoService.ObterServicosPorMotoIdAsync(moto.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal("Não existem serviços para esta moto", result.Error);
    }

    // ========================================
    // CREATE
    // ========================================

    [Fact(DisplayName = "AdicionarServicoAsync - Deve adicionar novo serviço com sucesso")]
    public async Task AdicionarServicoAsync_DeveAdicionarServico()
    {
        var dto = new ServicoDTO("Troca de óleo", StatusServico.EmAndamento, 10, 2);
        var entity = dto.ToServicoEntity();

        _servicoRepositoryMock
            .Setup(r => r.AdicionarServicoAsync(It.IsAny<ServicoEntity>()))
            .ReturnsAsync(entity);

        var result = await _servicoService.AdicionarServicoAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto.Descricao, result.Value!.Descricao);
    }

    // ========================================
    // UPDATE
    // ========================================

    [Fact(DisplayName = "EditarServicoAsync - Deve editar serviço existente com sucesso")]
    public async Task EditarServicoAsync_DeveEditarServico()
    {
        var servico = BuildServico();
        var dto = new ServicoDTO("Troca de óleo", StatusServico.EmAndamento, 10, 2);
        var atualizado = dto.ToServicoEntity();
        atualizado.Id = servico.Id;

        _servicoRepositoryMock.Setup(r => r.ObterServicoPorIdAsync(servico.Id)).ReturnsAsync(servico);
        _servicoRepositoryMock.Setup(r => r.EditarServicoAsync(servico.Id, It.IsAny<ServicoEntity>())).ReturnsAsync(atualizado);

        var result = await _servicoService.EditarServicoAsync(servico.Id, dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto.Descricao, result.Value!.Descricao);
    }

    [Fact(DisplayName = "EditarServicoAsync - Deve falhar se serviço não existe")]
    public async Task EditarServicoAsync_DeveFalhar_ServicoNaoExiste()
    {
        var dto = new ServicoDTO("Troca de óleo", StatusServico.EmAndamento, 10, 2);

        _servicoRepositoryMock.Setup(r => r.ObterServicoPorIdAsync(It.IsAny<long>())).ReturnsAsync((ServicoEntity?)null);

        var result = await _servicoService.EditarServicoAsync(99, dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado", result.Error);
    }

    // ========================================
    // DELETE
    // ========================================

    [Fact(DisplayName = "DeletarServicoAsync - Deve deletar serviço com sucesso")]
    public async Task DeletarServicoAsync_DeveDeletarServico()
    {
        var servico = BuildServico();

        _servicoRepositoryMock.Setup(r => r.ObterServicoPorIdAsync(servico.Id)).ReturnsAsync(servico);

        var result = await _servicoService.DeletarServicoAsync(servico.Id);

        _servicoRepositoryMock.Verify(r => r.DeletarServicoAsync(servico.Id), Times.Once);
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact(DisplayName = "DeletarServicoAsync - Deve falhar se serviço não existe")]
    public async Task DeletarServicoAsync_DeveFalhar_ServicoNaoExiste()
    {
        _servicoRepositoryMock.Setup(r => r.ObterServicoPorIdAsync(It.IsAny<long>())).ReturnsAsync((ServicoEntity?)null);

        var result = await _servicoService.DeletarServicoAsync(5);

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado", result.Error);
    }
}
