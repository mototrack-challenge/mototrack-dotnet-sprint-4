using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;

namespace MT.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    #region :: INJEÇÃO DE DEPENDÊNCIA

    private readonly HealthCheckService _healthService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthService = healthCheckService;
    }

    #endregion

    #region :: LIVE

    [HttpGet("live")]
    [SwaggerOperation(
        Summary = "Verifica se a aplicação está viva",
        Description = "Retorna o status de disponibilidade básica da aplicação (healthcheck do tipo 'live')."
    )]
    [SwaggerResponse(statusCode: 200, description: "A aplicação está viva e respondendo.")]
    [SwaggerResponse(statusCode: 503, description: "A aplicação não está saudável (falha em algum serviço interno).")]
    public async Task<IActionResult> Live(CancellationToken ct)
    {
        var report = await _healthService.CheckHealthAsync(
            r => r.Tags.Contains("live"), ct);

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                error = e.Value.Exception?.Message
            })
        };

        return report.Status == HealthStatus.Healthy
            ? Ok(result)
            : StatusCode(503, result);
    }

    #endregion

    #region :: READY

    [HttpGet("ready")]
    [SwaggerOperation(
        Summary = "Verifica se a aplicação está pronta para uso",
        Description = "Executa verificações mais completas (como banco de dados e dependências externas) para garantir que a aplicação esteja pronta para receber requisições."
    )]
    [SwaggerResponse(statusCode: 200, description: "A aplicação está pronta para uso.")]
    [SwaggerResponse(statusCode: 503, description: "A aplicação não está pronta (alguma dependência falhou).")]
    public async Task<IActionResult> Ready(CancellationToken ct)
    {
        var report = await _healthService.CheckHealthAsync(
            r => r.Tags.Contains("ready"), ct);

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                error = e.Value.Exception?.Message
            })
        };

        return report.Status == HealthStatus.Healthy
            ? Ok(result)
            : StatusCode(503, result);
    }

    #endregion
}
