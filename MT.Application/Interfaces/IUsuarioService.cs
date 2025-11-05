using MT.Application.Dtos;
using MT.Domain.Entities;

namespace MT.Application.Interfaces;

public interface IUsuarioService
{
    Task<OperationResult<UsuarioEntity?>> AutenticarUserAsync(AutenticacaoDTO autenticacao);
    Task<OperationResult<UsuarioEntity?>> AdicionarUsuarioAsync(UsuarioDTO usuarioDTO);
}
