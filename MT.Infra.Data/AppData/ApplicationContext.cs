using Microsoft.EntityFrameworkCore;
using MT.Domain.Entities;

namespace MT.Infra.Data.AppData;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MotoEntity>(entity =>
        {
            entity.Property(e => e.Modelo).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();
        });

        modelBuilder.Entity<ServicoEntity>(entity =>
        {
            entity.Property(s => s.Status).HasConversion<string>();
            entity.HasIndex(s => s.MotoId);
            entity.HasIndex(s => s.ColaboradorId);
        });
    }

    public DbSet<MotoEntity> Moto { get; set; }
    public DbSet<ColaboradorEntity> Colaborador { get; set; }
    public DbSet<ServicoEntity> Servico { get; set; }
    public DbSet<PecaEntity> Peca { get; set; }
    public DbSet<UsuarioEntity> Usuario { get; set; }
}
