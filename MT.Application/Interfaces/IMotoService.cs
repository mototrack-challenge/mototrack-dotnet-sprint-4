using MT.Domain.Entities;

namespace MT.Application.Interfaces;

public interface IMotoService
{
    Task<OperationResult<PageResultModel<IEnumerable<MotoEntity>>>> ObterTodasMotosAsync(int deslocamento = 0, int registrosRetornados = 10);
    Task<OperationResult<MotoEntity?>> ObterMotoPorIdAsync(long id);
}
