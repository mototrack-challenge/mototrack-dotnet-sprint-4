using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MT.Infra.Data.AppData;

namespace MT.Infra.Data.HealthCheck;

public class OracleHealthCheck : IHealthCheck
{
    private readonly ApplicationContext _context;

    public OracleHealthCheck(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

            return canConnect
            ? HealthCheckResult.Healthy("EF (Oracle) respondeu à consulta.")
            : HealthCheckResult.Unhealthy("EF (Oracle) falhou ao consultar.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro no HealthCheck: " + ex);
            return HealthCheckResult.Unhealthy("EF (Oracle) falhou ao consultar.", ex);
        }
    }
}
