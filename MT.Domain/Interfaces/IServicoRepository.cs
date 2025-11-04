using MT.Domain.Entities;

namespace MT.Domain.Interfaces;

public interface IServicoRepository
{
    Task<PageResultModel<IEnumerable<ServicoEntity>>> ObterTodosServicosAsync(int deslocamento = 0, int registrosRetornados = 10);
    Task<ServicoEntity?> ObterServicoPorIdAsync(long id);
    Task<IEnumerable<ServicoEntity>> ObterServicosPorMotoIdAsync(long motoId);
    Task<ServicoEntity?> AdicionarServicoAsync(ServicoEntity servico);
    Task<ServicoEntity?> EditarServicoAsync(long id, ServicoEntity novoServico);
    Task<ServicoEntity?> DeletarServicoAsync(long id);
}
