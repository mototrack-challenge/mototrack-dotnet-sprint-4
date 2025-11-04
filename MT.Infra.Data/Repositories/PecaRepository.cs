using Microsoft.EntityFrameworkCore;
using MT.Domain.Entities;
using MT.Domain.Interfaces;
using MT.Infra.Data.AppData;

namespace MT.Infra.Data.Repositories;

public class PecaRepository : IPecaRepository
{
    private readonly ApplicationContext _context;

    public PecaRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<PageResultModel<IEnumerable<PecaEntity>>> ObterTodasPecasAsync(int deslocamento = 0, int registrosRetornados = 10)
    {
        var totalRegistros = await _context.Peca.CountAsync();

        var result = await _context
            .Peca
            .OrderBy(c => c.Id)
            .Skip(deslocamento)
            .Take(registrosRetornados)
            .ToListAsync();

        return new PageResultModel<IEnumerable<PecaEntity>>
        {
            Data = result,
            Deslocamento = deslocamento,
            RegistrosRetornados = registrosRetornados,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<PecaEntity?> ObterPecaPorIdAsync(long id)
    {
        var result = await _context
            .Peca
            .FirstOrDefaultAsync(c => c.Id == id);

        return result;
    }

    public async Task<PecaEntity?> AdicionarPecaAsync(PecaEntity peca)
    {
        _context.Peca.Add(peca);
        await _context.SaveChangesAsync();

        return peca;
    }

    public async Task<PecaEntity?> EditarPecaAsync(long id, PecaEntity novaPeca)
    {
        var pecaExistente = await _context.Peca.FirstOrDefaultAsync(c => c.Id == id);

        if (pecaExistente is null)
            return null;

        pecaExistente.Nome = novaPeca.Nome;
        pecaExistente.Descricao = novaPeca.Descricao;
        pecaExistente.Codigo = novaPeca.Codigo;
        pecaExistente.QuantidadeEstoque = novaPeca.QuantidadeEstoque;

        await _context.SaveChangesAsync();
        return pecaExistente;
    }

    public async Task<PecaEntity?> DeletarPecaAsync(long id)
    {
        var peca = await _context.Peca.FindAsync(id);

        if (peca is null)
            return null;

        _context.Peca.Remove(peca);
        await _context.SaveChangesAsync();
        return peca;
    }
}
