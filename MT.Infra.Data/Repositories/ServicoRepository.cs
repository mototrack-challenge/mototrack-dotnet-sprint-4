using Microsoft.EntityFrameworkCore;
using MT.Domain.Entities;
using MT.Domain.Interfaces;
using MT.Infra.Data.AppData;

namespace MT.Infra.Data.Repositories;

public class ServicoRepository : IServicoRepository
{
    private readonly ApplicationContext _context;

    public ServicoRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<PageResultModel<IEnumerable<ServicoEntity>>> ObterTodosServicosAsync(int deslocamento = 0, int registrosRetornados = 10)
    {
        var totalRegistros = await _context.Servico.CountAsync();

        var result = await _context
            .Servico
            .Include(s => s.Moto)
            .Include(s => s.Colaborador)
            .OrderBy(c => c.Id)
            .Skip(deslocamento)
            .Take(registrosRetornados)
            .ToListAsync();

        return new PageResultModel<IEnumerable<ServicoEntity>>
        {
            Data = result,
            Deslocamento = deslocamento,
            RegistrosRetornados = registrosRetornados,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<ServicoEntity?> ObterServicoPorIdAsync(long id)
    {
        return await _context.Servico
            .Include(s => s.Moto)
            .Include(s => s.Colaborador)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<ServicoEntity>> ObterServicosPorMotoIdAsync(long motoId)
    {
        return await _context.Servico
            .Where(s => s.MotoId == motoId)
            .Include(s => s.Moto)
            .Include(s => s.Colaborador)
            .OrderBy(s => s.DataCadastro)
            .ToListAsync();
    }

    public async Task<ServicoEntity?> AdicionarServicoAsync(ServicoEntity servico)
    {
        _context.Servico.Add(servico);
        await _context.SaveChangesAsync();
        return servico;
    }

    public async Task<ServicoEntity?> EditarServicoAsync(long id, ServicoEntity novoServico)
    {
        var existente = await _context.Servico.FindAsync(id);

        if (existente == null)
            return null;

        existente.Descricao = novoServico.Descricao;
        existente.MotoId = novoServico.MotoId;
        existente.ColaboradorId = novoServico.ColaboradorId;
        existente.Status = novoServico.Status;

        await _context.SaveChangesAsync();

        return existente;
    }

    public async Task<ServicoEntity?> DeletarServicoAsync(long id)
    {
        var existente = await _context.Servico.FindAsync(id);

        if (existente == null)
            return null;

        _context.Servico.Remove(existente);
        await _context.SaveChangesAsync();

        return existente;
    }
}
