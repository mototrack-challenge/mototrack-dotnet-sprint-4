using MT.Application.Dtos;
using MT.Domain.Entities;

namespace MT.Application.Interfaces;

public interface IColaboradorService
{
    Task<OperationResult<PageResultModel<IEnumerable<ColaboradorEntity>>>> ObterTodosColaboradoresAsync(int deslocamento = 0, int registrosRetornados = 10);
    Task<OperationResult<ColaboradorEntity?>> ObterColaboradorPorIdAsync(long id);
    Task<OperationResult<ColaboradorEntity?>> AdicionarColaboradorAsync(ColaboradorDTO colaboradorDTO);
    Task<OperationResult<ColaboradorEntity?>> EditarColaboradorAsync(long id, ColaboradorDTO novoColaboradorDTO);
    Task<OperationResult<ColaboradorEntity?>> DeletarColaboradorAsync(long id);
}
