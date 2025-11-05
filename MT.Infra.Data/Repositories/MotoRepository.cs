using Microsoft.EntityFrameworkCore;
using MT.Domain.Entities;
using MT.Domain.Interfaces;
using MT.Infra.Data.AppData;

namespace MT.Infra.Data.Repositories;

public class MotoRepository : IMotoRepository
{
    #region :: INJEÇÃO DE DEPENDÊNCIA

    private readonly ApplicationContext _context;

    public MotoRepository(ApplicationContext context)
    {
        _context = context;
    }

    #endregion

    #region :: READ

    public async Task<PageResultModel<IEnumerable<MotoEntity>>> ObterTodasMotosAsync(int deslocamento = 0, int registrosRetornados = 10)
    {
        var totalRegistros = await _context.Moto.CountAsync();

        var result = await _context
            .Moto
            .Include(m => m.Servicos)
                .ThenInclude(s => s.Colaborador)
            .OrderBy(m => m.Id)
            .Skip(deslocamento)
            .Take(registrosRetornados)
            .ToListAsync();

        return new PageResultModel<IEnumerable<MotoEntity>>
        {
            Data = result,
            Deslocamento = deslocamento,
            RegistrosRetornados = registrosRetornados,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<MotoEntity?> ObterMotoPorIdAsync(long id)
    {
        var result = await _context
            .Moto
            .Include(m => m.Servicos)
                .ThenInclude(s => s.Colaborador)
            .FirstOrDefaultAsync(m => m.Id == id);

        return result;
    }

    #endregion
}
