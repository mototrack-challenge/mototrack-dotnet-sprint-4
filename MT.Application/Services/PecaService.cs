using MT.Application.Dtos;
using MT.Application.Mappers;
using MT.Domain.Entities;
using MT.Domain.Interfaces;
using MT.Application.Interfaces;
using System.Net;

namespace MT.Application.Services;

public class PecaService : IPecaService
{
    private readonly IPecaRepository _pecaRepository;

    public PecaService(IPecaRepository pecaRepository)
    {
        _pecaRepository = pecaRepository;
    }

    public async Task<OperationResult<PageResultModel<IEnumerable<PecaEntity>>>> ObterTodasPecasAsync(int deslocamento = 0, int registrosRetornados = 10)
    {
        try
        {
            var result = await _pecaRepository.ObterTodasPecasAsync(deslocamento, registrosRetornados);

            if (!result.Data.Any())
                return OperationResult<PageResultModel<IEnumerable<PecaEntity>>>.Failure("Não existe conteudo para peças", (int)HttpStatusCode.NotFound);

            return OperationResult<PageResultModel<IEnumerable<PecaEntity>>>.Success(result);
        }
        catch (Exception)
        {
            return OperationResult<PageResultModel<IEnumerable<PecaEntity>>>.Failure("Ocorreu um erro ao buscar as peças");
        }
    }

    public async Task<OperationResult<PecaEntity?>> ObterPecaPorIdAsync(long id)
    {
        try
        {
            var result = await _pecaRepository.ObterPecaPorIdAsync(id);

            if (result is null)
                return OperationResult<PecaEntity?>.Failure("Peça não encontrada", (int)HttpStatusCode.NotFound);

            return OperationResult<PecaEntity?>.Success(result);
        }
        catch (Exception)
        {
            return OperationResult<PecaEntity?>.Failure("Ocorreu um erro ao buscar a peça");
        }
    }

    public async Task<OperationResult<PecaEntity?>> AdicionarPecaAsync(PecaDTO pecaDTO)
    {
        try
        {
            var result = await _pecaRepository.AdicionarPecaAsync(pecaDTO.ToPecaEntity());

            return OperationResult<PecaEntity?>.Success(result);
        }
        catch (Exception)
        {
            return OperationResult<PecaEntity?>.Failure("Ocorreu um erro ao salvar a peça");
        }
    }

    public async Task<OperationResult<PecaEntity?>> EditarPecaAsync(long id, PecaDTO novaPecaDTO)
    {
        try
        {
            var existente = await _pecaRepository.ObterPecaPorIdAsync(id);

            if (existente is null) return OperationResult<PecaEntity?>.Failure("Peça não encontrada", (int)HttpStatusCode.NotFound);

            var entidade = novaPecaDTO.ToPecaEntity(); 
            entidade.Id = id;
            var atualizado = await _pecaRepository.EditarPecaAsync(id, entidade);

            return OperationResult<PecaEntity?>.Success(atualizado);
        }
        catch (Exception ex)
        {
            return OperationResult<PecaEntity?>.Failure("Ocorreu um erro ao editar a peça");
        }
    }

    public async Task<OperationResult<PecaEntity?>> DeletarPecaAsync(long id)
    {
        try
        {
            var existente = await _pecaRepository.ObterPecaPorIdAsync(id);

            if (existente is null)
                return OperationResult<PecaEntity?>.Failure("Peça não encontrada", (int)HttpStatusCode.NotFound);

            await _pecaRepository.DeletarPecaAsync(id);

            return OperationResult<PecaEntity?>.Success(null);
        }
        catch (Exception)
        {
            return OperationResult<PecaEntity?>.Failure("Ocorreu um erro ao deletar a peça");
        }
    }
}
