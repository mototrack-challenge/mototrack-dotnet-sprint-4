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

    #region :: CREATE

    public async Task<OperationResult<UsuarioEntity?>> AdicionarUsuarioAsync(UsuarioDTO usuarioDTO)
    {
        try
        {
            //var usuarios = await _usuarioRepository.ObterTodosUsuariosAsync();
            //var jaExiste = usuarios.Data.Any(u => u.Email == usuarioDTO.Email);

            //if (jaExiste)
            //    return OperationResult<UsuarioEntity?>.Failure(
            //        "Já existe um usuário com este e-mail.",
            //        (int)HttpStatusCode.Conflict
            //    );

            var result = await _usuarioRepository.AdicionarUsuarioAsync(usuarioDTO.ToUsuarioEntity());
            return OperationResult<UsuarioEntity?>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<UsuarioEntity?>.Failure("Ocorreu um erro ao salvar o usuário: " + ex.Message);
        }
    }

    #endregion
}
