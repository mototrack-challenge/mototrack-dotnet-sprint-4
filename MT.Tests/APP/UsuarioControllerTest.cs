using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MT.Application.Dtos;
using MT.Application.Interfaces;
using MT.Domain.Entities;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Net.Http.Json;

namespace MT.Tests.APP;

// Handler de autenticação fake
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string Scheme = "TestAuth";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Role, "admin"),
            };

        var identity = new ClaimsIdentity(claims, Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

// Factory customizada para o ambiente de testes
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IUsuarioService> UsuarioServiceMock { get; } = new();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Substitui o service real pelo mock
            services.RemoveAll(typeof(IUsuarioService));
            services.AddSingleton(UsuarioServiceMock.Object);

            // Autenticação fake
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                options.DefaultChallengeScheme = TestAuthHandler.Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });
        });
    }
}

public class UsuarioControllerTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UsuarioControllerTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "GET /api/usuario - Deve retornar lista de usuários")]
    [Trait("Controller", "Usuario")]
    public async Task Get_DeveRetornarListaDeUsuarios()
    {
        // Arrange
        var usuarios = new List<UsuarioEntity>
            {
                new UsuarioEntity { Id = 1, Nome = "Felipe", Email = "felipe@email.com", Senha = "123" },
                new UsuarioEntity { Id = 2, Nome = "Maria", Email = "maria@email.com", Senha = "abc" }
            };

        var pageResult = new PageResultModel<IEnumerable<UsuarioEntity>>
        {
            Data = usuarios,
            Deslocamento = 0,
            RegistrosRetornados = 2,
            TotalRegistros = 2
        };

        var retorno = OperationResult<PageResultModel<IEnumerable<UsuarioEntity>>>.Success(pageResult, (int)HttpStatusCode.OK);

        _factory.UsuarioServiceMock
            .Setup(s => s.ObterTodosUsuariosAsync(0, 10))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/usuario");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/usuario/{id} - Deve retornar um usuário por ID")]
    [Trait("Controller", "Usuario")]
    public async Task GetId_DeveRetornarUsuarioPorId()
    {
        // Arrange
        var usuario = new UsuarioEntity { Id = 1, Nome = "Felipe", Email = "felipe@email.com", Senha = "123" };

        var retorno = OperationResult<UsuarioEntity?>.Success(usuario, (int)HttpStatusCode.OK);

        _factory.UsuarioServiceMock
            .Setup(s => s.ObterUsuarioPorIdAsync(1))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/usuario/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "POST /api/usuario - Deve cadastrar novo usuário")]
    [Trait("Controller", "Usuario")]
    public async Task Post_DeveCadastrarUsuario()
    {
        // Arrange
        var dto = new UsuarioDTO("Felipe", "felipe@email.com", "123");

        var entity = new UsuarioEntity { Id = 1, Nome = dto.Nome, Email = dto.Email, Senha = dto.Senha };

        var retorno = OperationResult<UsuarioEntity?>.Success(entity, (int)HttpStatusCode.OK);

        _factory.UsuarioServiceMock
            .Setup(s => s.AdicionarUsuarioAsync(It.IsAny<UsuarioDTO>()))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/usuario", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
