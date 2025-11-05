using MT.Domain.Entities;

namespace MT.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<UsuarioEntity?> AutenticarAsync(string email, string senha);
    Task<UsuarioEntity?> AdicionarUsuarioAsync(UsuarioEntity usuario);
}
