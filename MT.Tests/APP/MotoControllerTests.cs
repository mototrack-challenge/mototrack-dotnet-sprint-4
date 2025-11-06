using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MT.Application.Interfaces;
using MT.Domain.Entities;
using MT.Domain.Enums;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Net.Http.Json;

namespace MT.Tests.APP;

// ================================
// HANDLER DE AUTENTICAÇÃO FAKE
// ================================
public class MotoTestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string Scheme = "TestAuth";

    public MotoTestAuthHandler(
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

// ================================
// FACTORY CUSTOMIZADA
// ================================
public class MotoWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IMotoService> MotoServiceMock { get; } = new();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove a implementação real e substitui pelo mock
            services.RemoveAll(typeof(IMotoService));
            services.AddSingleton(MotoServiceMock.Object);

            // Autenticação fake
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = MotoTestAuthHandler.Scheme;
                options.DefaultChallengeScheme = MotoTestAuthHandler.Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, MotoTestAuthHandler>(MotoTestAuthHandler.Scheme, _ => { });
        });
    }
}

// ================================
// TESTES DO MOTO CONTROLLER
// ================================
public class MotoControllerTests : IClassFixture<MotoWebApplicationFactory>
{
    private readonly MotoWebApplicationFactory _factory;

    public MotoControllerTests(MotoWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "GET /api/moto - Deve retornar lista de motos")]
    [Trait("Controller", "Moto")]
    public async Task Get_DeveRetornarListaDeMotos()
    {
        // Arrange
        var motos = new List<MotoEntity>
        {
            new MotoEntity { Id = 1, Placa = "ABC1234", Modelo = ModeloMoto.MOTTU_POP, Chassi = "XYZ123456", Status = StatusMoto.AVALIACAO },
            new MotoEntity { Id = 2, Placa = "DEF5678", Modelo = ModeloMoto.MOTTU_SPORT, Chassi = "XYZ789012", Status = StatusMoto.MANUTENCAO }
        };

        var pageResult = new PageResultModel<IEnumerable<MotoEntity>>
        {
            Data = motos,
            Deslocamento = 0,
            RegistrosRetornados = 2,
            TotalRegistros = 2
        };

        var retorno = OperationResult<PageResultModel<IEnumerable<MotoEntity>>>.Success(pageResult, (int)HttpStatusCode.OK);

        _factory.MotoServiceMock
            .Setup(s => s.ObterTodasMotosAsync(0, 10))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/moto");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/moto/{id} - Deve retornar uma moto por ID")]
    [Trait("Controller", "Moto")]
    public async Task GetId_DeveRetornarMotoPorId()
    {
        // Arrange
        var moto = new MotoEntity
        {
            Id = 1,
            Placa = "XYZ1234",
            Modelo = ModeloMoto.MOTTU_E,
            Chassi = "CHS123456789",
            Status = StatusMoto.AVALIACAO
        };

        var retorno = OperationResult<MotoEntity?>.Success(moto, (int)HttpStatusCode.OK);

        _factory.MotoServiceMock
            .Setup(s => s.ObterMotoPorIdAsync(1))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/moto/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/moto/{id} - Deve retornar 404 se moto não existir")]
    [Trait("Controller", "Moto")]
    public async Task GetId_DeveRetornarNotFound_SeMotoNaoExistir()
    {
        // Arrange
        var retorno = OperationResult<MotoEntity?>.Failure("Moto não encontrada", (int)HttpStatusCode.NotFound);

        _factory.MotoServiceMock
            .Setup(s => s.ObterMotoPorIdAsync(999))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/moto/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
