using Microsoft.EntityFrameworkCore;
using MT.Domain.Entities;
using MT.Infra.Data.AppData;
using MT.Infra.Data.Repositories;
using MT.Domain.Enums;

namespace MT.Tests.APP;

public class ServicoRepositoryTests
{
    private static ApplicationContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ApplicationContext(options);
    }

    private static ServicoEntity BuildServico(
        string descricao = "Troca de óleo",
        long motoId = 1,
        long colaboradorId = 1,
        StatusServico status = StatusServico.EmAndamento)
    {
        return new ServicoEntity
        {
            Descricao = descricao,
            MotoId = motoId,
            ColaboradorId = colaboradorId,
            Status = status,
            DataCadastro = DateTime.UtcNow
        };
    }

    // Helpers para criar Moto e Colaborador no contexto para que Includes funcionem
    private static MotoEntity BuildMoto(long id, string placa = "ABC1234", ModeloMoto modelo = ModeloMoto.MOTTU_POP, StatusMoto status = StatusMoto.DISPONIVEL)
    {
        return new MotoEntity
        {
            Id = id,
            Placa = placa,
            Chassi = "CHASSI12345678901",
            Modelo = modelo,
            Status = status
        };
    }

    private static ColaboradorEntity BuildColaborador(long id, string nome = "João", string matricula = "MAT001", string email = "joao@teste.com")
    {
        return new ColaboradorEntity
        {
            Id = id,
            Nome = nome,
            Matricula = matricula,
            Email = email
        };
    }

    [Fact(DisplayName = "AdicionarServicoAsync - Deve adicionar um novo serviço com sucesso")]
    public async Task AdicionarServicoAsync_DeveAdicionarServico()
    {
        using var context = CreateContext("AddServicoDB");
        var repo = new ServicoRepository(context);
        // cria entidades relacionadas para o include resolver
        context.Moto.Add(BuildMoto(1));
        context.Colaborador.Add(BuildColaborador(1));
        await context.SaveChangesAsync();

        var servico = BuildServico();

        var result = await repo.AdicionarServicoAsync(servico);

        Assert.NotNull(result);
        Assert.Equal(servico.Descricao, result.Descricao);
        Assert.Single(context.Servico);
    }

    [Fact(DisplayName = "ObterTodosServicosAsync - Deve retornar todos os serviços cadastrados com paginação")]
    public async Task ObterTodosServicosAsync_DeveRetornarServicos()
    {
        using var context = CreateContext("GetAllServicosDB");
        var repo = new ServicoRepository(context);

        // adiciona motos/colaboradores referenciados para as includes
        context.Moto.Add(BuildMoto(1));
        context.Moto.Add(BuildMoto(2, "BBB2222"));
        context.Colaborador.Add(BuildColaborador(1));
        context.Colaborador.Add(BuildColaborador(2, "Maria", "MAT002", "maria@teste.com"));

        context.Servico.AddRange(
            BuildServico("Revisão geral", 1, 1),
            BuildServico("Troca de pneus", 2, 2)
        );
        await context.SaveChangesAsync();

        var result = await repo.ObterTodosServicosAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalRegistros);
        Assert.Equal(2, result.Data.Count());
        // verificar que includes retornaram objetos preenchidos
        Assert.All(result.Data, s => Assert.NotNull(s.Moto));
        Assert.All(result.Data, s => Assert.NotNull(s.Colaborador));
    }

    [Fact(DisplayName = "ObterServicoPorIdAsync - Deve retornar o serviço pelo ID")]
    public async Task ObterServicoPorIdAsync_DeveRetornarServico()
    {
        using var context = CreateContext("GetServicoByIdDB");
        var repo = new ServicoRepository(context);

        // cria referências
        context.Moto.Add(BuildMoto(1));
        context.Colaborador.Add(BuildColaborador(1));
        await context.SaveChangesAsync();

        var servico = BuildServico("Troca de pastilhas", motoId: 1, colaboradorId: 1);
        context.Servico.Add(servico);
        await context.SaveChangesAsync();

        var result = await repo.ObterServicoPorIdAsync(servico.Id);

        Assert.NotNull(result);
        Assert.Equal(servico.Descricao, result.Descricao);
        Assert.NotNull(result.Moto);
        Assert.NotNull(result.Colaborador);
    }

    [Fact(DisplayName = "ObterServicosPorMotoIdAsync - Deve retornar todos os serviços de uma moto específica")]
    public async Task ObterServicosPorMotoIdAsync_DeveRetornarServicosDaMoto()
    {
        using var context = CreateContext("GetServicosByMotoDB");
        var repo = new ServicoRepository(context);

        // cria moto/colaborador de referência
        context.Moto.Add(BuildMoto(10, "MOTO10"));
        context.Moto.Add(BuildMoto(20, "MOTO20"));
        context.Colaborador.Add(BuildColaborador(1));
        await context.SaveChangesAsync();

        var servico1 = BuildServico("Troca de óleo", motoId: 10, colaboradorId: 1);
        var servico2 = BuildServico("Troca de pneus", motoId: 10, colaboradorId: 1);
        var servico3 = BuildServico("Lavagem completa", motoId: 20, colaboradorId: 1);

        context.Servico.AddRange(servico1, servico2, servico3);
        await context.SaveChangesAsync();

        var result = await repo.ObterServicosPorMotoIdAsync(10);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, s => Assert.Equal(10, s.MotoId));
        // includes resolvidos
        Assert.All(result, s => Assert.NotNull(s.Moto));
        Assert.All(result, s => Assert.NotNull(s.Colaborador));
    }

    [Fact(DisplayName = "EditarServicoAsync - Deve atualizar os dados de um serviço existente")]
    public async Task EditarServicoAsync_DeveAtualizarServico()
    {
        using var context = CreateContext("EditServicoDB");
        var repo = new ServicoRepository(context);

        // cria referencias
        context.Moto.Add(BuildMoto(1));
        context.Colaborador.Add(BuildColaborador(1));
        await context.SaveChangesAsync();

        // cria servico inicial com status Pendente
        var servico = BuildServico("Troca de óleo", motoId: 1, colaboradorId: 1, status: StatusServico.Pendente);
        context.Servico.Add(servico);
        await context.SaveChangesAsync();

        // novo com status Concluido -> esperamos que seja atualizado para Concluido
        var novo = BuildServico("Troca de óleo e filtro", motoId: servico.MotoId, colaboradorId: servico.ColaboradorId, status: StatusServico.Concluido);
        var result = await repo.EditarServicoAsync(servico.Id, novo);

        Assert.NotNull(result);
        Assert.Equal("Troca de óleo e filtro", result.Descricao);
        Assert.Equal(StatusServico.Concluido, result.Status); // *corrigido*: comparar com o status que foi setado no "novo"
    }

    [Fact(DisplayName = "EditarServicoAsync - Deve retornar null se o serviço não existir")]
    public async Task EditarServicoAsync_DeveRetornarNull_SeNaoExistir()
    {
        using var context = CreateContext("EditServicoNotFoundDB");
        var repo = new ServicoRepository(context);

        var novo = BuildServico("Atualizado");

        var result = await repo.EditarServicoAsync(999, novo);

        Assert.Null(result);
    }

    [Fact(DisplayName = "DeletarServicoAsync - Deve remover serviço existente")]
    public async Task DeletarServicoAsync_DeveRemoverServico()
    {
        using var context = CreateContext("DeleteServicoDB");
        var repo = new ServicoRepository(context);

        context.Moto.Add(BuildMoto(1));
        context.Colaborador.Add(BuildColaborador(1));
        await context.SaveChangesAsync();

        var servico = BuildServico("Troca de pneus", motoId: 1, colaboradorId: 1);
        context.Servico.Add(servico);
        await context.SaveChangesAsync();

        var result = await repo.DeletarServicoAsync(servico.Id);

        Assert.NotNull(result);
        Assert.Empty(context.Servico);
    }

    [Fact(DisplayName = "DeletarServicoAsync - Deve retornar null se o serviço não existir")]
    public async Task DeletarServicoAsync_DeveRetornarNull_SeNaoExistir()
    {
        using var context = CreateContext("DeleteServicoNotFoundDB");
        var repo = new ServicoRepository(context);

        var result = await repo.DeletarServicoAsync(999);

        Assert.Null(result);
    }
}
