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
using MT.Domain.Enums;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Net.Http.Json;


namespace MT.Tests.APP;

// Handler de autenticação fake
public class TestAuthHandlerServico : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string Scheme = "TestAuth";

    public TestAuthHandlerServico(
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
public class CustomServicoWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IServicoService> ServicoServiceMock { get; } = new();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Substitui o service real pelo mock
            services.RemoveAll(typeof(IServicoService));
            services.AddSingleton(ServicoServiceMock.Object);

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

public class ServicoControllerTest : IClassFixture<CustomServicoWebApplicationFactory>
{
    private readonly CustomServicoWebApplicationFactory _factory;

    public ServicoControllerTest(CustomServicoWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "GET /api/servico - Deve retornar lista de serviços")]
    [Trait("Controller", "Servico")]
    public async Task Get_DeveRetornarListaDeServicos()
    {
        var servicos = new List<ServicoEntity>
        {
            new() { Id = 1, Descricao = "Troca de óleo", Status = StatusServico.EmAndamento, MotoId = 10 },
            new() { Id = 2, Descricao = "Revisão geral", Status = StatusServico.Concluido, MotoId = 11 }
        };

        var page = new PageResultModel<IEnumerable<ServicoEntity>>
        {
            Data = servicos,
            Deslocamento = 0,
            RegistrosRetornados = 2,
            TotalRegistros = 2
        };

        var retorno = OperationResult<PageResultModel<IEnumerable<ServicoEntity>>>.Success(page, (int)HttpStatusCode.OK);

        _factory.ServicoServiceMock
            .Setup(s => s.ObterTodosServicosAsync(0, 10))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/servico");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/servico/{id} - Deve retornar serviço por ID")]
    [Trait("Controller", "Servico")]
    public async Task GetId_DeveRetornarServicoPorId()
    {
        var servico = new ServicoEntity { Id = 1, Descricao = "Troca de óleo", Status = StatusServico.EmAndamento, MotoId = 10 };

        var retorno = OperationResult<ServicoEntity?>.Success(servico, (int)HttpStatusCode.OK);

        _factory.ServicoServiceMock
            .Setup(s => s.ObterServicoPorIdAsync(1))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/servico/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "POST /api/servico - Deve cadastrar novo serviço")]
    [Trait("Controller", "Servico")]
    public async Task Post_DeveCadastrarServico()
    {
        var dto = new ServicoDTO("Troca de óleo", StatusServico.EmAndamento, 10, 1);
        var entity = new ServicoEntity { Id = 1, Descricao = dto.Descricao, Status = dto.Status, MotoId = dto.MotoId };

        var retorno = OperationResult<ServicoEntity?>.Success(entity, (int)HttpStatusCode.OK);

        _factory.ServicoServiceMock
            .Setup(s => s.AdicionarServicoAsync(It.IsAny<ServicoDTO>()))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/servico", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "PUT /api/servico/{id} - Deve atualizar serviço existente")]
    [Trait("Controller", "Servico")]
    public async Task Put_DeveAtualizarServico()
    {
        var dto = new ServicoDTO("Revisão geral", StatusServico.Concluido, 10, 1);
        var entity = new ServicoEntity { Id = 1, Descricao = dto.Descricao, Status = dto.Status, MotoId = dto.MotoId };

        var retorno = OperationResult<ServicoEntity?>.Success(entity, (int)HttpStatusCode.OK);

        _factory.ServicoServiceMock
            .Setup(s => s.EditarServicoAsync(1, It.IsAny<ServicoDTO>()))
            .ReturnsAsync(retorno);

        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/servico/1", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "DELETE /api/servico/{id} - Deve remover serviço")]
    [Trait("Controller", "Servico")]
    public async Task Delete_DeveRemoverServico()
    {
        var retorno = OperationResult<object?>.Success(null, (int)HttpStatusCode.OK);

        var retornoDelete = OperationResult<ServicoEntity?>.Success(null, (int)HttpStatusCode.OK);
        _factory.ServicoServiceMock
            .Setup(s => s.DeletarServicoAsync(1))
            .Returns(Task.FromResult(retornoDelete));

        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/api/servico/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
