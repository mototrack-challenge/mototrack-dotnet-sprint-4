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

// ========================================
// HANDLER DE AUTENTICAÇÃO FAKE
// ========================================
public class TestAuthHandlerColaborador : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string Scheme = "TestAuth";

    public TestAuthHandlerColaborador(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "tester"),
            new Claim(ClaimTypes.Role, "admin")
        };

        var identity = new ClaimsIdentity(claims, Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

// ========================================
// CUSTOM FACTORY PARA TESTES DO COLABORADOR
// ========================================
public class CustomWebApplicationFactoryColaborador : WebApplicationFactory<Program>
{
    public Mock<IColaboradorService> ColaboradorServiceMock { get; } = new();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Substitui o service real pelo mock
            services.RemoveAll(typeof(IColaboradorService));
            services.AddSingleton(ColaboradorServiceMock.Object);

            // Autenticação fake
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandlerColaborador.Scheme;
                options.DefaultChallengeScheme = TestAuthHandlerColaborador.Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandlerColaborador>(
                TestAuthHandlerColaborador.Scheme, _ => { });
        });
    }
}

// ========================================
// TESTES DO CONTROLLER COLABORADOR
// ========================================
public class ColaboradorControllerTest : IClassFixture<CustomWebApplicationFactoryColaborador>
{
    private readonly CustomWebApplicationFactoryColaborador _factory;

    public ColaboradorControllerTest(CustomWebApplicationFactoryColaborador factory)
    {
        _factory = factory;
    }

    // ========================================
    // GET
    // ========================================

    [Fact(DisplayName = "GET /api/colaborador - Deve retornar lista de colaboradores")]
    [Trait("Controller", "Colaborador")]
    public async Task Get_DeveRetornarListaDeColaboradores()
    {
        var colaboradores = new List<ColaboradorEntity>
        {
            new ColaboradorEntity { Id = 1, Nome = "João", Matricula = "001", Email = "joao@email.com" },
            new ColaboradorEntity { Id = 2, Nome = "Maria", Matricula = "002", Email = "maria@email.com" }
        };

        var page = new PageResultModel<IEnumerable<ColaboradorEntity>>
        {
            Data = colaboradores,
            Deslocamento = 0,
            RegistrosRetornados = 2,
            TotalRegistros = 2
        };

        var retorno = OperationResult<PageResultModel<IEnumerable<ColaboradorEntity>>>.Success(page, (int)HttpStatusCode.OK);

        _factory.ColaboradorServiceMock
            .Setup(s => s.ObterTodosColaboradoresAsync(0, 10))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/colaborador");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/colaborador/{id} - Deve retornar colaborador por ID")]
    [Trait("Controller", "Colaborador")]
    public async Task GetId_DeveRetornarColaboradorPorId()
    {
        var colaborador = new ColaboradorEntity
        {
            Id = 1,
            Nome = "João",
            Matricula = "001",
            Email = "joao@email.com"
        };

        var retorno = OperationResult<ColaboradorEntity?>.Success(colaborador, (int)HttpStatusCode.OK);

        _factory.ColaboradorServiceMock
            .Setup(s => s.ObterColaboradorPorIdAsync(1))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/colaborador/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ========================================
    // POST
    // ========================================

    [Fact(DisplayName = "POST /api/colaborador - Deve cadastrar novo colaborador")]
    [Trait("Controller", "Colaborador")]
    public async Task Post_DeveCadastrarColaborador()
    {
        var dto = new ColaboradorDTO("João", "001", "joao@email.com");
        var entity = new ColaboradorEntity { Id = 1, Nome = dto.Nome, Matricula = dto.Matricula, Email = dto.Email };

        var retorno = OperationResult<ColaboradorEntity?>.Success(entity, (int)HttpStatusCode.OK);

        _factory.ColaboradorServiceMock
            .Setup(s => s.AdicionarColaboradorAsync(It.IsAny<ColaboradorDTO>()))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/colaborador", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ========================================
    // PUT
    // ========================================

    [Fact(DisplayName = "PUT /api/colaborador/{id} - Deve editar colaborador com sucesso")]
    [Trait("Controller", "Colaborador")]
    public async Task Put_DeveEditarColaborador()
    {
        var dto = new ColaboradorDTO("João Atualizado", "001", "joao@email.com");
        var entity = new ColaboradorEntity { Id = 1, Nome = dto.Nome, Matricula = dto.Matricula, Email = dto.Email };

        var retorno = OperationResult<ColaboradorEntity?>.Success(entity, (int)HttpStatusCode.OK);

        _factory.ColaboradorServiceMock
            .Setup(s => s.EditarColaboradorAsync(1, It.IsAny<ColaboradorDTO>()))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/colaborador/1", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ========================================
    // DELETE
    // ========================================

    [Fact(DisplayName = "DELETE /api/colaborador/{id} - Deve deletar colaborador com sucesso")]
    [Trait("Controller", "Colaborador")]
    public async Task Delete_DeveDeletarColaborador()
    {
        var retorno = OperationResult<ColaboradorEntity?>.Success(null, (int)HttpStatusCode.OK);

        _factory.ColaboradorServiceMock
            .Setup(s => s.DeletarColaboradorAsync(1))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/api/colaborador/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
