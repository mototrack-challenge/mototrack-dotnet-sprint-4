using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MT.Infra.Data.AppData;
using MT.Domain.Interfaces;
using MT.Infra.Data.Repositories;
using MT.Application.Interfaces;
using MT.Application.Services;

namespace MT.Infra.IoC;

public class Bootstrap
{
    public static void AddIoC(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options => {
            //options.UseOracle(configuration.GetConnectionString("Oracle"));
            options.UseOracle(configuration.GetConnectionString("Oracle"));
        });

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
