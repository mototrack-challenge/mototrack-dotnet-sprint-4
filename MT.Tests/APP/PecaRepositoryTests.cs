using Microsoft.EntityFrameworkCore;
using MT.Domain.Entities;
using MT.Infra.Data.AppData;
using MT.Infra.Data.Repositories;

namespace MT.Tests.APP;

public class PecaRepositoryTests
{
    private static ApplicationContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ApplicationContext(options);
    }

    private static PecaEntity BuildPeca(string nome = "Parafuso", string descricao = "Parafuso de aço", string codigo = "P001", int quantidade = 50)
    {
        return new PecaEntity
        {
            Nome = nome,
            Descricao = descricao,
            Codigo = codigo,
            QuantidadeEstoque = quantidade
        };
    }

    [Fact(DisplayName = "AdicionarPecaAsync - Deve adicionar uma nova peça com sucesso")]
    public async Task AdicionarPecaAsync_DeveAdicionarPeca()
    {
        using var context = CreateContext("AddPecaDB");
        var repo = new PecaRepository(context);
        var peca = BuildPeca();

        var result = await repo.AdicionarPecaAsync(peca);

        Assert.NotNull(result);
        Assert.Equal(peca.Nome, result.Nome);
        Assert.Single(context.Peca);
    }

    [Fact(DisplayName = "ObterTodasPecasAsync - Deve retornar todas as peças cadastradas")]
    public async Task ObterTodasPecasAsync_DeveRetornarPecas()
    {
        using var context = CreateContext("GetAllPecasDB");
        var repo = new PecaRepository(context);

        context.Peca.AddRange(
            BuildPeca("Parafuso", "Peça 1", "P001", 10),
            BuildPeca("Porca", "Peça 2", "P002", 20)
        );
        await context.SaveChangesAsync();

        var result = await repo.ObterTodasPecasAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalRegistros);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact(DisplayName = "ObterPecaPorIdAsync - Deve retornar uma peça existente pelo ID")]
    public async Task ObterPecaPorIdAsync_DeveRetornarPeca()
    {
        using var context = CreateContext("GetPecaByIdDB");
        var repo = new PecaRepository(context);

        var peca = BuildPeca("Arruela", "Peça pequena", "P003", 5);
        context.Peca.Add(peca);
        await context.SaveChangesAsync();

        var result = await repo.ObterPecaPorIdAsync(peca.Id);

        Assert.NotNull(result);
        Assert.Equal(peca.Codigo, result.Codigo);
    }

    [Fact(DisplayName = "EditarPecaAsync - Deve atualizar dados de uma peça existente")]
    public async Task EditarPecaAsync_DeveAtualizarPeca()
    {
        using var context = CreateContext("EditPecaDB");
        var repo = new PecaRepository(context);

        var peca = BuildPeca("Pino", "Peça antiga", "P004", 15);
        context.Peca.Add(peca);
        await context.SaveChangesAsync();

        var nova = BuildPeca("Pino Atualizado", "Peça nova", "P004A", 30);
        var result = await repo.EditarPecaAsync(peca.Id, nova);

        Assert.NotNull(result);
        Assert.Equal("Pino Atualizado", result.Nome);
        Assert.Equal(30, result.QuantidadeEstoque);
    }

    [Fact(DisplayName = "EditarPecaAsync - Deve retornar null se a peça não existir")]
    public async Task EditarPecaAsync_DeveRetornarNull_QuandoNaoExistir()
    {
        using var context = CreateContext("EditPecaNotFoundDB");
        var repo = new PecaRepository(context);

        var nova = BuildPeca("Nova Peça", "Inexistente", "PX", 5);

        var result = await repo.EditarPecaAsync(999, nova);

        Assert.Null(result);
    }

    [Fact(DisplayName = "DeletarPecaAsync - Deve remover uma peça existente")]
    public async Task DeletarPecaAsync_DeveRemoverPeca()
    {
        using var context = CreateContext("DeletePecaDB");
        var repo = new PecaRepository(context);

        var peca = BuildPeca("Porca", "Peça de teste", "P005", 10);
        context.Peca.Add(peca);
        await context.SaveChangesAsync();

        var result = await repo.DeletarPecaAsync(peca.Id);

        Assert.NotNull(result);
        Assert.Empty(context.Peca);
    }

    [Fact(DisplayName = "DeletarPecaAsync - Deve retornar null se a peça não existir")]
    public async Task DeletarPecaAsync_DeveRetornarNull_QuandoNaoExistir()
    {
        using var context = CreateContext("DeletePecaNotFoundDB");
        var repo = new PecaRepository(context);

        var result = await repo.DeletarPecaAsync(999);

        Assert.Null(result);
    }
}
