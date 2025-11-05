using MT.Application.Dtos;
using MT.Application.Mappers;
using MT.Domain.Entities;
using MT.Domain.Interfaces;
using MT.Application.Interfaces;
using System.Net;

namespace MT.Application.Services;

public class ColaboradorService : IColaboradorService
{
    #region :: INJEÇÃO DE DEPENDÊNCIA

    private readonly IColaboradorRepository _colaboradorRepository;

    public ColaboradorService(IColaboradorRepository colaboradorRepository)
    {
        _colaboradorRepository = colaboradorRepository;
    }

    #endregion

    #region :: READ

    public async Task<OperationResult<PageResultModel<IEnumerable<ColaboradorEntity>>>> ObterTodosColaboradoresAsync(int deslocamento = 0, int registrosRetornados = 10)
    {
        try
        {
            var result = await _colaboradorRepository.ObterTodosColaboradoresAsync(deslocamento, registrosRetornados);

            if (!result.Data.Any())
                return OperationResult<PageResultModel<IEnumerable<ColaboradorEntity>>>.Failure("Não existe conteudo para colaborador", (int)HttpStatusCode.NotFound);

            return OperationResult<PageResultModel<IEnumerable<ColaboradorEntity>>>.Success(result);
        }
        catch (Exception)
        {
            return OperationResult<PageResultModel<IEnumerable<ColaboradorEntity>>>.Failure("Ocorreu um erro ao buscar os colaboradores");
        }
    }

    public async Task<OperationResult<ColaboradorEntity?>> ObterColaboradorPorIdAsync(long id)
    {
        try
        {
            var result = await _colaboradorRepository.ObterColaboradorPorIdAsync(id);

            if (result is null)
                return OperationResult<ColaboradorEntity?>.Failure("Colaborador não encontrado", (int)HttpStatusCode.NotFound);

            return OperationResult<ColaboradorEntity?>.Success(result);
        }
        catch (Exception)
        {
            return OperationResult<ColaboradorEntity?>.Failure("Ocorreu um erro ao buscar o colaborador");
        }
    }

    #endregion

    #region :: CREATE

    public async Task<OperationResult<ColaboradorEntity?>> AdicionarColaboradorAsync(ColaboradorDTO colaboradorDTO)
    {
        try
        {
            var existe = await _colaboradorRepository.ObterTodosColaboradoresAsync();
            var jaExiste = existe.Data.Any(c =>
                c.Email == colaboradorDTO.Email || c.Matricula == colaboradorDTO.Matricula);

            if (jaExiste)
                return OperationResult<ColaboradorEntity?>.Failure(
                    "Já existe um colaborador com este e-mail ou matrícula",
                    (int)HttpStatusCode.Conflict
                );

            var result = await _colaboradorRepository.AdicionarColaboradorAsync(colaboradorDTO.ToColaboradorEntity());
            return OperationResult<ColaboradorEntity?>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<ColaboradorEntity?>.Failure("Ocorreu um erro ao salvar o colaborador: " + ex.Message);
        }
    }

    #endregion

    #region :: UPDATE

    public async Task<OperationResult<ColaboradorEntity?>> EditarColaboradorAsync(long id, ColaboradorDTO novoColaboradorDTO)
    {
        try
        {
            var existente = await _colaboradorRepository.ObterColaboradorPorIdAsync(id);

            if (existente is null)
                return OperationResult<ColaboradorEntity?>.Failure("Colaborador não encontrado", (int)HttpStatusCode.NotFound);

            if (await _colaboradorRepository.ExisteOutroComMesmoEmailAsync(id, novoColaboradorDTO.Email))
                return OperationResult<ColaboradorEntity?>.Failure("Já existe outro colaborador com este e-mail", (int)HttpStatusCode.Conflict);

            if (await _colaboradorRepository.ExisteOutroComMesmoMatriculaAsync(id, novoColaboradorDTO.Matricula))
                return OperationResult<ColaboradorEntity?>.Failure("Já existe outro colaborador com esta matrícula", (int)HttpStatusCode.Conflict);

            var entidade = novoColaboradorDTO.ToColaboradorEntity();
            entidade.Id = id;

            var atualizado = await _colaboradorRepository.EditarColaboradorAsync(id, entidade);

            return OperationResult<ColaboradorEntity?>.Success(atualizado);
        }
        catch (Exception ex)
        {
            return OperationResult<ColaboradorEntity?>.Failure("Ocorreu um erro ao editar o colaborador: " + ex.Message);
        }
    }

    #endregion

    #region :: DELETE

    public async Task<OperationResult<ColaboradorEntity?>> DeletarColaboradorAsync(long id)
    {
        try
        {
            var existente = await _colaboradorRepository.ObterColaboradorPorIdAsync(id);

            if (existente is null)
                return OperationResult<ColaboradorEntity?>.Failure("Colaborador não encontrado", (int)HttpStatusCode.NotFound);

            await _colaboradorRepository.DeletarColaboradorAsync(id);

            return OperationResult<ColaboradorEntity?>.Success(null);
        }
        catch (Exception)
        {
            return OperationResult<ColaboradorEntity?>.Failure("Ocorreu um erro ao deletar o colaborador");
        }
    }

    #endregion
}
