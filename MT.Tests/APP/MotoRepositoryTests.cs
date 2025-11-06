using Microsoft.EntityFrameworkCore;
using MT.Domain.Entities;
using MT.Domain.Enums;
using MT.Infra.Data.AppData;
using MT.Infra.Data.Repositories;

namespace MT.Tests.APP;

public class MotoRepositoryTests
{
    private static ApplicationContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ApplicationContext(options);
    }

    // 🏍️ Cria uma moto válida conforme as restrições do modelo
    private static MotoEntity BuildMoto(
        string placa = "ABC1234",
        string chassi = "CHASSI12345678901",
        ModeloMoto modelo = ModeloMoto.MOTTU_POP,
        StatusMoto status = StatusMoto.DISPONIVEL)
    {
        return new MotoEntity
        {
            Placa = placa,
            Chassi = chassi,
            Modelo = modelo,
            Status = status
        };
    }

    [Fact(DisplayName = "ObterTodasMotosAsync - Deve retornar todas as motos cadastradas")]
    public async Task ObterTodasMotosAsync_DeveRetornarTodasMotos()
    {
        // Arrange
        using var context = CreateContext("GetAllMotosDB");
        var repo = new MotoRepository(context);

        var moto1 = BuildMoto("AAA1111", "CHASSI00000000001", ModeloMoto.MOTTU_POP);
        var moto2 = BuildMoto("BBB2222", "CHASSI00000000002", ModeloMoto.MOTTU_SPORT);

        context.Moto.AddRange(moto1, moto2);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.ObterTodasMotosAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalRegistros);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact(DisplayName = "ObterMotoPorIdAsync - Deve retornar uma moto pelo ID")]
    public async Task ObterMotoPorIdAsync_DeveRetornarMotoCorreta()
    {
        // Arrange
        using var context = CreateContext("GetMotoByIdDB");
        var repo = new MotoRepository(context);

        var moto = BuildMoto("CCC3333", "CHASSI00000000003", ModeloMoto.MOTTU_E);
        context.Moto.Add(moto);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.ObterMotoPorIdAsync(moto.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(moto.Placa, result.Placa);
        Assert.Equal(moto.Chassi, result.Chassi);
        Assert.Equal(moto.Modelo, result.Modelo);
    }

    [Fact(DisplayName = "ObterMotoPorIdAsync - Deve retornar nulo se a moto não existir")]
    public async Task ObterMotoPorIdAsync_DeveRetornarNulo()
    {
        // Arrange
        using var context = CreateContext("GetMotoNotFoundDB");
        var repo = new MotoRepository(context);

        // Act
        var result = await repo.ObterMotoPorIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}
