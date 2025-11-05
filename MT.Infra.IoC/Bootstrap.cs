using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MT.Infra.Data.AppData;
using MT.Domain.Interfaces;
using MT.Infra.Data.Repositories;
using MT.Application.Interfaces;
using MT.Application.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MT.Infra.Data.HealthCheck;

namespace MT.Infra.IoC;

public class Bootstrap
{
    public static void AddIoC(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options => {
            //options.UseOracle(configuration.GetConnectionString("Oracle"));
            options.UseOracle(configuration.GetConnectionString("Oracle"));
        });

        services.AddHealthChecks()
            // Liveness: verifica api “estou no ar”
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
            //Readiness: verifica se o mongo "esta online"
            .AddCheck<OracleHealthCheck>("oracle_ef_query", tags: new[] { "ready" });

        services.AddTransient<IMotoRepository, MotoRepository>();
        services.AddTransient<IMotoService, MotoService>();

        services.AddTransient<IColaboradorRepository, ColaboradorRepository>();
        services.AddTransient<IColaboradorService, ColaboradorService>();

        services.AddTransient<IServicoRepository, ServicoRepository>();
        services.AddTransient<IServicoService, ServicoService>();

        services.AddTransient<IPecaRepository, PecaRepository>();
        services.AddTransient<IPecaService, PecaService>();

        services.AddTransient<IUsuarioRepository, UsuarioRepository>();
        services.AddTransient<IUsuarioService, UsuarioService>();
    }
}
