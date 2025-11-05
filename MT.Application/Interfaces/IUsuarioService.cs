using MT.Application.Dtos;
using MT.Domain.Entities;

namespace MT.Application.Interfaces;

public interface IUsuarioService
{
    Task<OperationResult<UsuarioEntity?>> AutenticarUserAsync(AutenticacaoDTO autenticacao);
    Task<OperationResult<PageResultModel<IEnumerable<UsuarioEntity>>>> ObterTodosUsuariosAsync(int deslocamento = 0, int registrosRetornados = 10);
    Task<OperationResult<UsuarioEntity?>> ObterUsuarioPorIdAsync(long id);
    Task<OperationResult<UsuarioEntity?>> AdicionarUsuarioAsync(UsuarioDTO usuarioDTO);
    Task<OperationResult<UsuarioEntity?>> EditarUsuarioAsync(long id, UsuarioDTO novoUsuarioDTO);
    Task<OperationResult<UsuarioEntity?>> DeletarUsuarioAsync(long id);
}
