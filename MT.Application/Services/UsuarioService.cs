using System.Net;
using MT.Application.Dtos;
using MT.Application.Interfaces;
using MT.Application.Mappers;
using MT.Domain.Entities;
using MT.Domain.Interfaces;

namespace MT.Application.Services;

public class UsuarioService : IUsuarioService
{
    #region :: INJEÇÃO DE DEPENDÊNCIA

    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    #endregion

    #region :: LOGIN

    public async Task<OperationResult<UsuarioEntity?>> AutenticarUserAsync(AutenticacaoDTO entity)
    {
        try
        {
            var userAuth = await _usuarioRepository.AutenticarAsync(entity.Email, entity.Senha);

            return OperationResult<UsuarioEntity?>.Success(userAuth);
        }
        catch (Exception)
        {
            return OperationResult<UsuarioEntity?>.Failure("Ocorreu um erro ao buscar o usuario");
        }
    }

    #endregion

    #region :: READ

    public async Task<OperationResult<PageResultModel<IEnumerable<UsuarioEntity>>>> ObterTodosUsuariosAsync(int deslocamento = 0, int registrosRetornados = 10)
    {
        try
        {
            var result = await _usuarioRepository.ObterTodosUsuariosAsync(deslocamento, registrosRetornados);

            if (!result.Data.Any())
                return OperationResult<PageResultModel<IEnumerable<UsuarioEntity>>>.Failure("Não existe conteudo para usuários", (int)HttpStatusCode.NotFound);

            return OperationResult<PageResultModel<IEnumerable<UsuarioEntity>>>.Success(result);
        }
        catch (Exception)
        {
            return OperationResult<PageResultModel<IEnumerable<UsuarioEntity>>>.Failure("Ocorreu um erro ao buscar os usuários");
        }
    }

    public async Task<OperationResult<UsuarioEntity?>> ObterUsuarioPorIdAsync(long id)
    {
        try
        {
            var result = await _usuarioRepository.ObterUsuarioPorIdAsync(id);

            if (result is null)
                return OperationResult<UsuarioEntity?>.Failure("Usuário não encontrado", (int)HttpStatusCode.NotFound);

            return OperationResult<UsuarioEntity?>.Success(result);
        }
        catch (Exception)
        {
            return OperationResult<UsuarioEntity?>.Failure("Ocorreu um erro ao buscar o usuário");
        }
    }

    #endregion

    #region :: CREATE

    public async Task<OperationResult<UsuarioEntity?>> AdicionarUsuarioAsync(UsuarioDTO usuarioDTO)
    {
        try
        {
            var usuarios = await _usuarioRepository.ObterTodosUsuariosAsync();
            var jaExiste = usuarios.Data.Any(u => u.Email == usuarioDTO.Email);

            if (jaExiste)
                return OperationResult<UsuarioEntity?>.Failure(
                    "Já existe um usuário com este e-mail.",
                    (int)HttpStatusCode.Conflict
                );

            var result = await _usuarioRepository.AdicionarUsuarioAsync(usuarioDTO.ToUsuarioEntity());
            return OperationResult<UsuarioEntity?>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<UsuarioEntity?>.Failure("Ocorreu um erro ao salvar o usuário: " + ex.Message);
        }
    }

    #endregion

    #region :: UPDATE

    public async Task<OperationResult<UsuarioEntity?>> EditarUsuarioAsync(long id, UsuarioDTO novoUsuarioDTO)
    {
        try
        {
            var existente = await _usuarioRepository.ObterUsuarioPorIdAsync(id);

            if (existente is null)
                return OperationResult<UsuarioEntity?>.Failure("Usuário não encontrado", (int)HttpStatusCode.NotFound);

            if (await _usuarioRepository.ExisteOutroComMesmoEmailAsync(id, novoUsuarioDTO.Email))
                return OperationResult<UsuarioEntity?>.Failure("Já existe outro usuário com este e-mail", (int)HttpStatusCode.Conflict);

            var entidade = novoUsuarioDTO.ToUsuarioEntity();
            entidade.Id = id;

            var atualizado = await _usuarioRepository.EditarUsuarioAsync(id, entidade);

            return OperationResult<UsuarioEntity?>.Success(atualizado);
        }
        catch (Exception ex)
        {
            return OperationResult<UsuarioEntity?>.Failure("Ocorreu um erro ao editar o usuário: " + ex.Message);
        }
    }

    #endregion

    #region :: DELETE

    public async Task<OperationResult<UsuarioEntity?>> DeletarUsuarioAsync(long id)
    {
        try
        {
            var existente = await _usuarioRepository.ObterUsuarioPorIdAsync(id);

            if (existente is null)
                return OperationResult<UsuarioEntity?>.Failure("Usuário não encontrado", (int)HttpStatusCode.NotFound);

            await _usuarioRepository.DeletarUsuarioAsync(id);

            return OperationResult<UsuarioEntity?>.Success(null);
        }
        catch (Exception)
        {
            return OperationResult<UsuarioEntity?>.Failure("Ocorreu um erro ao deletar o usuário");
        }
    }

    #endregion
}
