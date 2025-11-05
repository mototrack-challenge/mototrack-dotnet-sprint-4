using Microsoft.EntityFrameworkCore;
using MT.Domain.Entities;
using MT.Domain.Errors;
using MT.Domain.Interfaces;
using MT.Infra.Data.AppData;

namespace MT.Infra.Data.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    #region :: INJEÇÃO DE DEPENDÊNCIA

    private readonly ApplicationContext _context;

    public UsuarioRepository(ApplicationContext context)
    {
        _context = context;
    }

    #endregion

    #region :: LOGIN

    public async Task<UsuarioEntity?> AutenticarAsync(string email, string senha)
    {
        var userAuth = await _context.Usuario
            .FirstOrDefaultAsync(x => x.Email == email && x.Senha == senha);

        if (userAuth is null)
            throw new NoContentException("Usuario não encontrado");

        return userAuth;
    }

    #endregion

    #region :: CREATE

    public async Task<UsuarioEntity?> AdicionarUsuarioAsync(UsuarioEntity usuario)
    {
        _context.Usuario.Add(usuario);
        await _context.SaveChangesAsync();

        return usuario;
    }

    #endregion
}
