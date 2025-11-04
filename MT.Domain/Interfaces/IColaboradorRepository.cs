using MT.Domain.Entities;

namespace MT.Domain.Interfaces;

public interface IColaboradorRepository
{
    Task<PageResultModel<IEnumerable<ColaboradorEntity>>> ObterTodosColaboradoresAsync(int deslocamento = 0, int registrosRetornados = 10);
    Task<ColaboradorEntity?> ObterColaboradorPorIdAsync(long id);
    Task<ColaboradorEntity?> AdicionarColaboradorAsync(ColaboradorEntity colaborador);
    Task<ColaboradorEntity?> EditarColaboradorAsync(long id, ColaboradorEntity novoColaborador);
    Task<ColaboradorEntity?> DeletarColaboradorAsync(long id);
    Task<bool> ExisteOutroComMesmoEmailAsync(long id, string email);
    Task<bool> ExisteOutroComMesmoMatriculaAsync(long id, string matricula);
}
