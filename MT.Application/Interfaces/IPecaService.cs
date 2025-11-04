using MT.Application.Dtos;
using MT.Domain.Entities;

namespace MT.Application.Interfaces;

public interface IPecaService
{
    Task<OperationResult<PageResultModel<IEnumerable<PecaEntity>>>> ObterTodasPecasAsync(int deslocamento = 0, int registrosRetornados = 10);
    Task<OperationResult<PecaEntity?>> ObterPecaPorIdAsync(long id);
    Task<OperationResult<PecaEntity?>> AdicionarPecaAsync(PecaDTO pecaDTO);
    Task<OperationResult<PecaEntity?>> EditarPecaAsync(long id, PecaDTO novaPecaDTO);
    Task<OperationResult<PecaEntity?>> DeletarPecaAsync(long id);
}
