using Microsoft.EntityFrameworkCore;
using MT.Domain.Entities;
using MT.Domain.Errors;
using MT.Infra.Data.AppData;
using MT.Infra.Data.Repositories;

namespace MT.Tests.APP;

public class UsuarioRepositoryTests
{
    private static ApplicationContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ApplicationContext(options);
    }

    private static UsuarioEntity BuildUsuario(string nome = "Felipe Sora", string email = "felipe@email.com", string senha = "123456")
    {
        return new UsuarioEntity
        {
            Nome = nome,
            Email = email,
            Senha = senha
        };
    }

    [Fact(DisplayName = "AdicionarUsuarioAsync - Deve adicionar um novo usuário com sucesso")]
    public async Task AdicionarUsuarioAsync_DeveAdicionarUsuario()
    {
        using var context = CreateContext("AddUserDB");
        var repo = new UsuarioRepository(context);
        var usuario = BuildUsuario();

        var result = await repo.AdicionarUsuarioAsync(usuario);

        Assert.NotNull(result);
        Assert.Equal(usuario.Email, result.Email);
        Assert.Single(context.Usuario);
    }

    [Fact(DisplayName = "ObterTodosUsuariosAsync - Deve retornar todos os usuários cadastrados")]
    public async Task ObterTodosUsuariosAsync_DeveRetornarUsuarios()
    {
        using var context = CreateContext("GetAllUsersDB");
        var repo = new UsuarioRepository(context);

        context.Usuario.AddRange(
            BuildUsuario("User1", "user1@email.com", "123"),
            BuildUsuario("User2", "user2@email.com", "123")
        );
        await context.SaveChangesAsync();

        var result = await repo.ObterTodosUsuariosAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalRegistros);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact(DisplayName = "AutenticarAsync - Deve autenticar usuário com sucesso")]
    public async Task AutenticarAsync_DeveAutenticarUsuario()
    {
        using var context = CreateContext("AuthUserDB");
        var repo = new UsuarioRepository(context);

        var usuario = BuildUsuario("Felipe", "felipe@test.com", "senha123");
        context.Usuario.Add(usuario);
        await context.SaveChangesAsync();

        var result = await repo.AutenticarAsync("felipe@test.com", "senha123");

        Assert.NotNull(result);
        Assert.Equal(usuario.Email, result.Email);
    }

    [Fact(DisplayName = "AutenticarAsync - Deve lançar exceção se o usuário não existir")]
    public async Task AutenticarAsync_DeveLancarExcecao()
    {
        using var context = CreateContext("AuthUserNotFoundDB");
        var repo = new UsuarioRepository(context);

        await Assert.ThrowsAsync<NoContentException>(() =>
            repo.AutenticarAsync("inexistente@email.com", "senha123"));
    }

    [Fact(DisplayName = "EditarUsuarioAsync - Deve atualizar dados de um usuário existente")]
    public async Task EditarUsuarioAsync_DeveAtualizarUsuario()
    {
        using var context = CreateContext("EditUserDB");
        var repo = new UsuarioRepository(context);

        var usuario = BuildUsuario("User", "user@email.com", "123");
        context.Usuario.Add(usuario);
        await context.SaveChangesAsync();

        var novo = BuildUsuario("User Atualizado", "user@email.com", "321");
        var result = await repo.EditarUsuarioAsync(usuario.Id, novo);

        Assert.NotNull(result);
        Assert.Equal("User Atualizado", result.Nome);
    }

    [Fact(DisplayName = "DeletarUsuarioAsync - Deve remover usuário existente")]
    public async Task DeletarUsuarioAsync_DeveRemoverUsuario()
    {
        using var context = CreateContext("DeleteUserDB");
        var repo = new UsuarioRepository(context);

        var usuario = BuildUsuario("User", "user@email.com", "123");
        context.Usuario.Add(usuario);
        await context.SaveChangesAsync();

        var result = await repo.DeletarUsuarioAsync(usuario.Id);

        Assert.NotNull(result);
        Assert.Empty(context.Usuario);
    }

    [Fact(DisplayName = "ExisteOutroComMesmoEmailAsync - Deve retornar verdadeiro se já houver outro com o mesmo email")]
    public async Task ExisteOutroComMesmoEmailAsync_DeveDetectarEmailDuplicado()
    {
        using var context = CreateContext("EmailDuplicateDB");
        var repo = new UsuarioRepository(context);

        var usuario1 = BuildUsuario("User1", "email@teste.com", "123");
        var usuario2 = BuildUsuario("User2", "outro@teste.com", "321");

        context.Usuario.AddRange(usuario1, usuario2);
        await context.SaveChangesAsync();

        var existe = await repo.ExisteOutroComMesmoEmailAsync(usuario2.Id, "email@teste.com");

        Assert.True(existe);
    }
}
