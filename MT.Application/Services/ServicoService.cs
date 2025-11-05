using MT.Application.Dtos;
using MT.Application.Mappers;
using MT.Domain.Entities;
using MT.Domain.Interfaces;
using MT.Application.Interfaces;
using System.Net;

namespace MT.Application.Services;

public class ServicoService : IServicoService
{
    #region :: INJEÇÃO DE DEPENDÊNCIA

    private readonly IServicoRepository _servicoRepository;
    private readonly IMotoRepository _motoRepository;

    public ServicoService(IServicoRepository servicoRepository, IMotoRepository motoRepository)
    {
        _servicoRepository = servicoRepository;
        _motoRepository = motoRepository;
    }

    #endregion

    #region :: READ

    public async Task<OperationResult<PageResultModel<IEnumerable<ServicoEntity>>>> ObterTodosServicosAsync(int deslocamento = 0, int registrosRetornados = 10)
    {
        try
        {
            var result = await _servicoRepository.ObterTodosServicosAsync(deslocamento, registrosRetornados);

            if (!result.Data.Any())
                return OperationResult<PageResultModel<IEnumerable<ServicoEntity>>>.Failure("Não existe conteudo para serviços",(int)HttpStatusCode.NotFound);

            return OperationResult<PageResultModel<IEnumerable<ServicoEntity>>>.Success(result);
        }
        catch (Exception)
        {
            return OperationResult<PageResultModel<IEnumerable<ServicoEntity>>>.Failure("Ocorreu um erro ao buscar os serviços");
        }
    }

    public async Task<OperationResult<ServicoEntity?>> ObterServicoPorIdAsync(long id)
    {
        try
        {
            var result = await _servicoRepository.ObterServicoPorIdAsync(id);

            if (result is null)
                return OperationResult<ServicoEntity?>.Failure("Serviço não encontrado", (int)HttpStatusCode.NotFound);

            return OperationResult<ServicoEntity?>.Success(result);
        }
        catch (Exception)
        {
            return OperationResult<ServicoEntity?>.Failure("Ocorreu um erro ao buscar o serviço");
        }
    }

    public async Task<OperationResult<IEnumerable<ServicoEntity>>> ObterServicosPorMotoIdAsync(long motoId)
    {
        try
        {
            var moto = await _motoRepository.ObterMotoPorIdAsync(motoId);

            if (moto is null)
                return OperationResult<IEnumerable<ServicoEntity>>.Failure("Moto não encontrada", (int)HttpStatusCode.NotFound);

            var servicos = await _servicoRepository.ObterServicosPorMotoIdAsync(motoId);

            if (servicos == null || !servicos.Any())
                return OperationResult<IEnumerable<ServicoEntity>>.Failure("Não existem serviços para esta moto", (int)HttpStatusCode.NoContent);

            return OperationResult<IEnumerable<ServicoEntity>>.Success(servicos);
        }
        catch (Exception)
        {
            return OperationResult<IEnumerable<ServicoEntity>>.Failure("Ocorreu um erro ao buscar os serviços da moto");
        }
    }

    #endregion

    #region :: CREATE

    public async Task<OperationResult<ServicoEntity?>> AdicionarServicoAsync(ServicoDTO servicoDTO)
    {
        try
        {
            var result = await _servicoRepository.AdicionarServicoAsync(servicoDTO.ToServicoEntity());

            return OperationResult<ServicoEntity?>.Success(result);
        }
        catch (Exception)
        {
            return OperationResult<ServicoEntity?>.Failure("Ocorreu um erro ao salvar o serviço");
        }
    }

    #endregion

    #region :: UPDATE

    public async Task<OperationResult<ServicoEntity?>> EditarServicoAsync(long id, ServicoDTO novoServicoDTO)
    {
        try
        {
            var existente = await _servicoRepository.ObterServicoPorIdAsync(id);

            if (existente is null) return OperationResult<ServicoEntity?>.Failure("Serviço não encontrado", (int)HttpStatusCode.NotFound);

            var entidade = novoServicoDTO.ToServicoEntity();
            entidade.Id = id;
            var atualizado = await _servicoRepository.EditarServicoAsync(id, entidade);

            return OperationResult<ServicoEntity?>.Success(atualizado);
        }
        catch (Exception ex)
        {
            return OperationResult<ServicoEntity?>.Failure("Ocorreu um erro ao editar o serviço");
        }
    }

    #endregion

    #region :: DELETE

    public async Task<OperationResult<ServicoEntity?>> DeletarServicoAsync(long id)
    {
        try
        {
            var existente = await _servicoRepository.ObterServicoPorIdAsync(id);

            if (existente is null)
                return OperationResult<ServicoEntity?>.Failure("Serviço não encontrado", (int)HttpStatusCode.NotFound);

            await _servicoRepository.DeletarServicoAsync(id);

            return OperationResult<ServicoEntity?>.Success(null);
        }
        catch (Exception)
        {
            return OperationResult<ServicoEntity?>.Failure("Ocorreu um erro ao deletar o serviço");
        }
    }

    #endregion
}
