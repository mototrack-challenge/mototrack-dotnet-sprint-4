using MT.Application.Dtos;
using MT.Domain.Entities;

namespace MT.Application.Interfaces;

public interface IServicoService
{
    Task<OperationResult<PageResultModel<IEnumerable<ServicoEntity>>>> ObterTodosServicosAsync(int deslocamento = 0, int registrosRetornados = 10);
    Task<OperationResult<ServicoEntity?>> ObterServicoPorIdAsync(long id);
    Task<OperationResult<IEnumerable<ServicoEntity>>> ObterServicosPorMotoIdAsync(long motoId);
    Task<OperationResult<ServicoEntity?>> AdicionarServicoAsync(ServicoDTO servicoDTO);
    Task<OperationResult<ServicoEntity?>> EditarServicoAsync(long id, ServicoDTO novoServicoDTO);
    Task<OperationResult<ServicoEntity?>> DeletarServicoAsync(long id);
}
