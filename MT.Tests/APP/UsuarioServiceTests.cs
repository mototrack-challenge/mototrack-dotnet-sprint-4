using MT.Application.Dtos;
using MT.Application.Services;
using MT.Domain.Entities;
using MT.Domain.Errors;
using MT.Domain.Interfaces;
using Moq;
using MT.Application.Mappers;

namespace MT.Tests.APP;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly UsuarioService _usuarioService;

    public UsuarioServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _usuarioService = new UsuarioService(_usuarioRepositoryMock.Object);
    }

    private static UsuarioEntity BuildUsuario(long id = 1, string nome = "Felipe", string email = "felipe@email.com", string senha = "123")
    {
        return new UsuarioEntity
        {
            Id = id,
            Nome = nome,
            Email = email,
            Senha = senha
        };
    }

    // ========================================
    // LOGIN
    // ========================================

    [Fact(DisplayName = "AutenticarUserAsync - Deve autenticar usuário com sucesso")]
    public async Task AutenticarUserAsync_DeveRetornarSucesso()
    {
        var usuario = BuildUsuario();
        var dto = new AutenticacaoDTO(usuario.Email, usuario.Senha);

        _usuarioRepositoryMock
            .Setup(r => r.AutenticarAsync(usuario.Email, usuario.Senha))
            .ReturnsAsync(usuario);

        var result = await _usuarioService.AutenticarUserAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(usuario.Email, result.Value!.Email);
    }

    [Fact(DisplayName = "AutenticarUserAsync - Deve retornar falha em caso de erro")]
    public async Task AutenticarUserAsync_DeveRetornarFalha()
    {
        var dto = new AutenticacaoDTO("email@erro.com", "123");

        _usuarioRepositoryMock
            .Setup(r => r.AutenticarAsync(dto.Email, dto.Senha))
            .ThrowsAsync(new NoContentException("Usuário não encontrado"));

        var result = await _usuarioService.AutenticarUserAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Ocorreu um erro ao buscar o usuario", result.Error);
    }

    // ========================================
    // READ
    // ========================================

    [Fact(DisplayName = "ObterTodosUsuariosAsync - Deve retornar lista de usuários com sucesso")]
    public async Task ObterTodosUsuariosAsync_DeveRetornarUsuarios()
    {
        var usuarios = new List<UsuarioEntity> { BuildUsuario(), BuildUsuario(2, "User2", "user2@email.com", "abc") };

        var page = new PageResultModel<IEnumerable<UsuarioEntity>>
        {
            Data = usuarios,
            TotalRegistros = 2,
            Deslocamento = 0,
            RegistrosRetornados = 2
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterTodosUsuariosAsync(0, 10))
            .ReturnsAsync(page);

        var result = await _usuarioService.ObterTodosUsuariosAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.TotalRegistros);
    }

    [Fact(DisplayName = "ObterUsuarioPorIdAsync - Deve retornar usuário existente")]
    public async Task ObterUsuarioPorIdAsync_DeveRetornarUsuario()
    {
        var usuario = BuildUsuario();

        _usuarioRepositoryMock
            .Setup(r => r.ObterUsuarioPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        var result = await _usuarioService.ObterUsuarioPorIdAsync(usuario.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact(DisplayName = "ObterUsuarioPorIdAsync - Deve retornar falha se usuário não existir")]
    public async Task ObterUsuarioPorIdAsync_DeveFalhar_UsuarioNaoExiste()
    {
        _usuarioRepositoryMock
            .Setup(r => r.ObterUsuarioPorIdAsync(It.IsAny<long>()))
            .ReturnsAsync((UsuarioEntity?)null);

        var result = await _usuarioService.ObterUsuarioPorIdAsync(99);

        Assert.False(result.IsSuccess);
        Assert.Equal("Usuário não encontrado", result.Error);
    }

    // ========================================
    // CREATE
    // ========================================

    [Fact(DisplayName = "AdicionarUsuarioAsync - Deve adicionar novo usuário com sucesso")]
    public async Task AdicionarUsuarioAsync_DeveAdicionarUsuario()
    {
        var dto = new UsuarioDTO("Felipe", "novo@email.com", "123");
        var entity = dto.ToUsuarioEntity();

        _usuarioRepositoryMock
            .Setup(r => r.ObterTodosUsuariosAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new PageResultModel<IEnumerable<UsuarioEntity>> { Data = new List<UsuarioEntity>() });

        _usuarioRepositoryMock
            .Setup(r => r.AdicionarUsuarioAsync(It.IsAny<UsuarioEntity>()))
            .ReturnsAsync(entity);

        var result = await _usuarioService.AdicionarUsuarioAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal("novo@email.com", result.Value!.Email);
    }

    [Fact(DisplayName = "AdicionarUsuarioAsync - Deve falhar se já existir e-mail")]
    public async Task AdicionarUsuarioAsync_DeveFalhar_EmailExistente()
    {
        var dto = new UsuarioDTO("Felipe", "email@existe.com", "123");
        var usuarioExistente = new UsuarioEntity { Id = 1, Nome = "Outro", Email = "email@existe.com", Senha = "abc" };

        _usuarioRepositoryMock
            .Setup(r => r.ObterTodosUsuariosAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new PageResultModel<IEnumerable<UsuarioEntity>> { Data = new List<UsuarioEntity> { usuarioExistente } });

        var result = await _usuarioService.AdicionarUsuarioAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Já existe um usuário com este e-mail.", result.Error);
    }

    // ========================================
    // UPDATE
    // ========================================

    [Fact(DisplayName = "EditarUsuarioAsync - Deve editar usuário com sucesso")]
    public async Task EditarUsuarioAsync_DeveEditarUsuario()
    {
        var usuario = BuildUsuario();
        var dto = new UsuarioDTO("Atualizado", usuario.Email, "456");
        var atualizado = dto.ToUsuarioEntity();
        atualizado.Id = usuario.Id;

        _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorIdAsync(usuario.Id)).ReturnsAsync(usuario);
        _usuarioRepositoryMock.Setup(r => r.ExisteOutroComMesmoEmailAsync(usuario.Id, dto.Email)).ReturnsAsync(false);
        _usuarioRepositoryMock.Setup(r => r.EditarUsuarioAsync(usuario.Id, It.IsAny<UsuarioEntity>())).ReturnsAsync(atualizado);

        var result = await _usuarioService.EditarUsuarioAsync(usuario.Id, dto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Atualizado", result.Value!.Nome);
    }

    [Fact(DisplayName = "EditarUsuarioAsync - Deve falhar se usuário não existe")]
    public async Task EditarUsuarioAsync_DeveFalhar_UsuarioNaoExiste()
    {
        var dto = new UsuarioDTO("Novo", "teste@email.com", "123");

        _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorIdAsync(It.IsAny<long>())).ReturnsAsync((UsuarioEntity?)null);

        var result = await _usuarioService.EditarUsuarioAsync(99, dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Usuário não encontrado", result.Error);
    }

    // ========================================
    // DELETE
    // ========================================

    [Fact(DisplayName = "DeletarUsuarioAsync - Deve deletar usuário com sucesso")]
    public async Task DeletarUsuarioAsync_DeveDeletarUsuario()
    {
        var usuario = BuildUsuario();

        _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorIdAsync(usuario.Id)).ReturnsAsync(usuario);
        _usuarioRepositoryMock.Setup(r => r.DeletarUsuarioAsync(usuario.Id)).ReturnsAsync(usuario);

        var result = await _usuarioService.DeletarUsuarioAsync(usuario.Id);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact(DisplayName = "DeletarUsuarioAsync - Deve falhar se usuário não existe")]
    public async Task DeletarUsuarioAsync_DeveFalhar_UsuarioNaoExiste()
    {
        _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorIdAsync(It.IsAny<long>())).ReturnsAsync((UsuarioEntity?)null);

        var result = await _usuarioService.DeletarUsuarioAsync(5);

        Assert.False(result.IsSuccess);
        Assert.Equal("Usuário não encontrado", result.Error);
    }
}
