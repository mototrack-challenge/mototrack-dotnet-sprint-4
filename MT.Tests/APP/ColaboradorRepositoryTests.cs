using Microsoft.EntityFrameworkCore;
using MT.Domain.Entities;
using MT.Infra.Data.AppData;
using MT.Infra.Data.Repositories;

namespace MT.Tests.APP;

public class ColaboradorRepositoryTests
{
    private static ApplicationContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ApplicationContext(options);
    }

    private static ColaboradorEntity BuildColaborador(
        string nome = "Felipe Sora",
        string matricula = "12345",
        string email = "felipe@email.com")
    {
        return new ColaboradorEntity
        {
            Nome = nome,
            Matricula = matricula,
            Email = email
        };
    }

    [Fact(DisplayName = "AdicionarColaboradorAsync - Deve adicionar colaborador com sucesso")]
    public async Task AdicionarColaboradorAsync_DeveAdicionarComSucesso()
    {
        using var context = CreateContext("AddColabDB");
        var repo = new ColaboradorRepository(context);

        var colaborador = BuildColaborador();

        var result = await repo.AdicionarColaboradorAsync(colaborador);

        Assert.NotNull(result);
        Assert.Equal("Felipe Sora", result.Nome);
        Assert.Single(context.Colaborador);
    }

    [Fact(DisplayName = "ObterTodosColaboradoresAsync - Deve retornar todos os colaboradores")]
    public async Task ObterTodosColaboradoresAsync_DeveRetornarLista()
    {
        using var context = CreateContext("GetAllColabDB");
        var repo = new ColaboradorRepository(context);

        context.Colaborador.AddRange(
            BuildColaborador("User1", "001", "user1@email.com"),
            BuildColaborador("User2", "002", "user2@email.com")
        );
        await context.SaveChangesAsync();

        var result = await repo.ObterTodosColaboradoresAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalRegistros);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact(DisplayName = "ObterColaboradorPorIdAsync - Deve retornar colaborador por ID")]
    public async Task ObterColaboradorPorIdAsync_DeveRetornarColaborador()
    {
        using var context = CreateContext("GetColabByIdDB");
        var repo = new ColaboradorRepository(context);

        var colaborador = BuildColaborador("Felipe", "007", "felipe@teste.com");
        context.Colaborador.Add(colaborador);
        await context.SaveChangesAsync();

        var result = await repo.ObterColaboradorPorIdAsync(colaborador.Id);

        Assert.NotNull(result);
        Assert.Equal("Felipe", result.Nome);
    }

    [Fact(DisplayName = "EditarColaboradorAsync - Deve atualizar colaborador existente")]
    public async Task EditarColaboradorAsync_DeveAtualizarDados()
    {
        using var context = CreateContext("EditColabDB");
        var repo = new ColaboradorRepository(context);

        var colaborador = BuildColaborador("Antigo", "001", "antigo@teste.com");
        context.Colaborador.Add(colaborador);
        await context.SaveChangesAsync();

        var novo = BuildColaborador("Atualizado", "001", "novo@teste.com");

        var result = await repo.EditarColaboradorAsync(colaborador.Id, novo);

        Assert.NotNull(result);
        Assert.Equal("Atualizado", result.Nome);
        Assert.Equal("novo@teste.com", result.Email);
    }

    [Fact(DisplayName = "EditarColaboradorAsync - Deve retornar null se colaborador não existir")]
    public async Task EditarColaboradorAsync_DeveRetornarNullSeNaoExistir()
    {
        using var context = CreateContext("EditColabNullDB");
        var repo = new ColaboradorRepository(context);

        var novo = BuildColaborador("Inexistente", "999", "inexistente@teste.com");
        var result = await repo.EditarColaboradorAsync(99, novo);

        Assert.Null(result);
    }

    [Fact(DisplayName = "DeletarColaboradorAsync - Deve remover colaborador existente")]
    public async Task DeletarColaboradorAsync_DeveRemoverComSucesso()
    {
        using var context = CreateContext("DeleteColabDB");
        var repo = new ColaboradorRepository(context);

        var colaborador = BuildColaborador("User", "123", "user@teste.com");
        context.Colaborador.Add(colaborador);
        await context.SaveChangesAsync();

        var result = await repo.DeletarColaboradorAsync(colaborador.Id);

        Assert.NotNull(result);
        Assert.Empty(context.Colaborador);
    }

    [Fact(DisplayName = "DeletarColaboradorAsync - Deve retornar null se colaborador não existir")]
    public async Task DeletarColaboradorAsync_DeveRetornarNullSeNaoExistir()
    {
        using var context = CreateContext("DeleteColabNullDB");
        var repo = new ColaboradorRepository(context);

        var result = await repo.DeletarColaboradorAsync(999);

        Assert.Null(result);
    }

    [Fact(DisplayName = "ExisteOutroComMesmoEmailAsync - Deve detectar email duplicado")]
    public async Task ExisteOutroComMesmoEmailAsync_DeveDetectarDuplicado()
    {
        using var context = CreateContext("EmailDupDB");
        var repo = new ColaboradorRepository(context);

        var colab1 = BuildColaborador("User1", "001", "email@teste.com");
        var colab2 = BuildColaborador("User2", "002", "outro@teste.com");

        context.Colaborador.AddRange(colab1, colab2);
        await context.SaveChangesAsync();

        var existe = await repo.ExisteOutroComMesmoEmailAsync(colab2.Id, "email@teste.com");

        Assert.True(existe);
    }

    [Fact(DisplayName = "ExisteOutroComMesmoMatriculaAsync - Deve detectar matrícula duplicada")]
    public async Task ExisteOutroComMesmoMatriculaAsync_DeveDetectarDuplicado()
    {
        using var context = CreateContext("MatriculaDupDB");
        var repo = new ColaboradorRepository(context);

        var colab1 = BuildColaborador("User1", "001", "user1@teste.com");
        var colab2 = BuildColaborador("User2", "002", "user2@teste.com");

        context.Colaborador.AddRange(colab1, colab2);
        await context.SaveChangesAsync();

        var existe = await repo.ExisteOutroComMesmoMatriculaAsync(colab2.Id, "001");

        Assert.True(existe);
    }
}
