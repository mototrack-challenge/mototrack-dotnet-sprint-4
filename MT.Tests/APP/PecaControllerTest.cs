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
public class TestAuthHandlerPeca : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string Scheme = "TestAuth";

    public TestAuthHandlerPeca(
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
public class CustomPecaWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IPecaService> PecaServiceMock { get; } = new();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Substitui o service real pelo mock
            services.RemoveAll(typeof(IPecaService));
            services.AddSingleton(PecaServiceMock.Object);

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

public class PecaControllerTest : IClassFixture<CustomPecaWebApplicationFactory>
{
    private readonly CustomPecaWebApplicationFactory _factory;

    public PecaControllerTest(CustomPecaWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "GET /api/peca - Deve retornar lista de peças")]
    [Trait("Controller", "Peca")]
    public async Task Get_DeveRetornarListaDePecas()
    {
        // Arrange
        var pecas = new List<PecaEntity>
        {
            new PecaEntity { Id = 1, Nome = "Filtro de Óleo", Codigo = "F123", Descricao = "Filtro original", QuantidadeEstoque = 20 },
            new PecaEntity { Id = 2, Nome = "Correia Dentada", Codigo = "C456", Descricao = "Correia reforçada", QuantidadeEstoque = 10 }
        };

        var pageResult = new PageResultModel<IEnumerable<PecaEntity>>
        {
            Data = pecas,
            Deslocamento = 0,
            RegistrosRetornados = 2,
            TotalRegistros = 2
        };

        var retorno = OperationResult<PageResultModel<IEnumerable<PecaEntity>>>.Success(pageResult, (int)HttpStatusCode.OK);

        _factory.PecaServiceMock
            .Setup(s => s.ObterTodasPecasAsync(0, 10))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/peca");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/peca/{id} - Deve retornar uma peça por ID")]
    [Trait("Controller", "Peca")]
    public async Task GetId_DeveRetornarPecaPorId()
    {
        // Arrange
        var peca = new PecaEntity
        {
            Id = 1,
            Nome = "Filtro de Ar",
            Codigo = "A789",
            Descricao = "Filtro esportivo",
            QuantidadeEstoque = 15
        };

        var retorno = OperationResult<PecaEntity?>.Success(peca, (int)HttpStatusCode.OK);

        _factory.PecaServiceMock
            .Setup(s => s.ObterPecaPorIdAsync(1))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/peca/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "POST /api/peca - Deve cadastrar nova peça")]
    [Trait("Controller", "Peca")]
    public async Task Post_DeveCadastrarPeca()
    {
        // Arrange
        var dto = new PecaDTO("Pastilha de Freio", "F888", "Pastilha dianteira", 50);

        var entity = new PecaEntity
        {
            Id = 1,
            Nome = dto.Nome,
            Codigo = dto.Codigo,
            Descricao = dto.Descricao,
            QuantidadeEstoque = dto.QuantidadeEstoque
        };

        var retorno = OperationResult<PecaEntity?>.Success(entity, (int)HttpStatusCode.OK);

        _factory.PecaServiceMock
            .Setup(s => s.AdicionarPecaAsync(It.IsAny<PecaDTO>()))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/peca", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "PUT /api/peca/{id} - Deve atualizar uma peça existente")]
    [Trait("Controller", "Peca")]
    public async Task Put_DeveAtualizarPeca()
    {
        // Arrange
        var dto = new PecaDTO("Amortecedor Traseiro", "A321", "Amortecedor original", 5);

        var entity = new PecaEntity
        {
            Id = 1,
            Nome = dto.Nome,
            Codigo = dto.Codigo,
            Descricao = dto.Descricao,
            QuantidadeEstoque = dto.QuantidadeEstoque
        };

        var retorno = OperationResult<PecaEntity?>.Success(entity, (int)HttpStatusCode.OK);

        _factory.PecaServiceMock
            .Setup(s => s.EditarPecaAsync(1, It.IsAny<PecaDTO>()))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync("/api/peca/1", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "DELETE /api/peca/{id} - Deve remover uma peça existente")]
    [Trait("Controller", "Peca")]
    public async Task Delete_DeveRemoverPeca()
    {
        // Arrange
        var entity = new PecaEntity { Id = 1, Nome = "Cabo de Embreagem", Codigo = "E999", Descricao = "Peça original", QuantidadeEstoque = 2 };

        var retorno = OperationResult<PecaEntity?>.Success(entity, (int)HttpStatusCode.OK);

        _factory.PecaServiceMock
            .Setup(s => s.DeletarPecaAsync(1))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/peca/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
