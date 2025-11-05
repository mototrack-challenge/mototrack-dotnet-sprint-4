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

    #region :: READ

    public async Task<PageResultModel<IEnumerable<UsuarioEntity>>> ObterTodosUsuariosAsync(int deslocamento = 0, int registrosRetornados = 10)
    {
        var totalRegistros = await _context.Usuario.CountAsync();

        var result = await _context
            .Usuario
            .OrderBy(c => c.Id)
            .Skip(deslocamento)
            .Take(registrosRetornados)
            .ToListAsync();

        return new PageResultModel<IEnumerable<UsuarioEntity>>
        {
            Data = result,
            Deslocamento = deslocamento,
            RegistrosRetornados = registrosRetornados,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<UsuarioEntity?> ObterUsuarioPorIdAsync(long id)
    {
        var result = await _context
            .Usuario
            .FirstOrDefaultAsync(c => c.Id == id);

        return result;
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

    #region :: UPDATE

    public async Task<UsuarioEntity?> EditarUsuarioAsync(long id, UsuarioEntity novoUsuario)
    {
        var usuarioExistente = await _context.Usuario.FirstOrDefaultAsync(c => c.Id == id);

        if (usuarioExistente is null)
            return null;

        usuarioExistente.Nome = novoUsuario.Nome;
        usuarioExistente.Email = novoUsuario.Email;
        usuarioExistente.Senha = novoUsuario.Senha;

        await _context.SaveChangesAsync();
        return usuarioExistente;
    }

    #endregion

    #region :: DELETE
    public async Task<UsuarioEntity?> DeletarUsuarioAsync(long id)
    {
        var usuario = await _context.Usuario.FindAsync(id);

        if (usuario is null)
            return null;

        _context.Usuario.Remove(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    #endregion

    #region :: VALIDAÇÕES

    public async Task<bool> ExisteOutroComMesmoEmailAsync(long id, string email)
    {
        var existe = await _context.Usuario
            .Where(c => c.Email == email && c.Id != id)
            .FirstOrDefaultAsync();

        return existe != null;
    }

    #endregion
}
