using MT.Domain.Entities;

namespace MT.Domain.Interfaces;

public interface IMotoRepository
{
    Task<PageResultModel<IEnumerable<MotoEntity>>> ObterTodasMotosAsync(int deslocamento = 0, int registrosRetornados = 10);
    Task<MotoEntity?> ObterMotoPorIdAsync(long id);
}
