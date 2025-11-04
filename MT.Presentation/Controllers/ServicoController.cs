using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MT.Application.Dtos;
using MT.Application.Interfaces;
using MT.Domain.Entities;
using MT.Presentation.Doc.Samples;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Security.Cryptography;

namespace mototrack_backend_rest_dotnet.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServicoController : ControllerBase
{
    private readonly IServicoService _servicoService;

    public ServicoController(IServicoService servicoService)
    {
        _servicoService = servicoService;
    }

    [HttpGet]
    [SwaggerOperation(
            Summary = "Lista de serviços",
            Description = "Retorna a lista completa de serviços cadastrados."
        )]
    [SwaggerResponse(statusCode: 200, description: "Lista retornada com sucesso", type: typeof(IEnumerable<ServicoEntity>))]
    [SwaggerResponse(statusCode: 204, description: "Lista não tem dados")]
    [SwaggerResponseExample(statusCode: 200, typeof(ServicoResponseListSample))]
    [EnableRateLimiting("MotoTrack")]
    public async Task<IActionResult> Get(int deslocamento = 0, int registrosRetornados = 10)
    {
        var result = await _servicoService.ObterTodosServicosAsync(deslocamento, registrosRetornados);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        var hateaos = new
        {
            data = result.Value.Data.Select(s => new {
                s.Id,
                s.Descricao,
                s.DataCadastro,
                s.Status,
                s.MotoId,
                Colaborador = s.Colaborador != null ? new
                {
                    s.Colaborador.Id,
                    s.Colaborador.Nome,
                    s.Colaborador.Matricula,
                    s.Colaborador.Email
                } : null,
                links = new
                {
                    self = Url.Action(nameof(GetId), "Servico", new { id = s.Id }, Request.Scheme),
                    put = Url.Action(nameof(Put), "Servico", new { id = s.Id }, Request.Scheme),
                    delete = Url.Action(nameof(Delete), "Servico", new { id = s.Id }, Request.Scheme)
                }
            }),
            links = new
            {
                self = Url.Action(nameof(GetId), "Servico", null),
                post = Url.Action(nameof(Post), "Servico", null, Request.Scheme),
            },
            pagina = new
            {
                result.Value.Deslocamento,
                result.Value.RegistrosRetornados,
                result.Value.TotalRegistros
            }
        };

        return Ok(hateaos);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
            Summary = "Obtém um serviço pelo ID",
            Description = "Retorna o serviço correspondente ao ID informado."
        )]
    [SwaggerResponse(statusCode: 200, description: "Serviço encontrado", type: typeof(ServicoEntity))]
    [SwaggerResponse(statusCode: 404, description: "Serviço não encontrado")]
    [SwaggerResponseExample(statusCode: 200, typeof(ServicoResponseSample))]
    public async Task<IActionResult> GetId(long id)
    {
        var servico = await _servicoService.ObterServicoPorIdAsync(id);

        if (!servico.IsSuccess) return StatusCode(servico.StatusCode, servico.Error);

        var response = new
        {
            servico.Value.Id,
            servico.Value.Descricao,
            servico.Value.DataCadastro,
            servico.Value.Status,
            servico.Value.MotoId,
            Colaborador = servico.Value.Colaborador != null ? new
            {
                servico.Value.Colaborador.Id,
                servico.Value.Colaborador.Nome,
                servico.Value.Colaborador.Matricula,
                servico.Value.Colaborador.Email
            } : null,
            links = new
            {
                self = Url.Action(nameof(GetId), "Servico", new { id = servico.Value.Id }, Request.Scheme),
                put = Url.Action(nameof(Put), "Servico", new { id = servico.Value.Id }, Request.Scheme),
                delete = Url.Action(nameof(Delete), "Servico", new { id = servico.Value.Id }, Request.Scheme)
            }
        };

        return Ok(response);
    }

    [HttpGet("moto/{motoId}")]
    [SwaggerOperation(
    Summary = "Lista serviços de uma moto",
    Description = "Retorna todos os serviços vinculados a uma moto específica pelo ID."
)]
    [SwaggerResponse(statusCode: 200, description: "Lista retornada com sucesso", type: typeof(IEnumerable<ServicoEntity>))]
    [SwaggerResponse(statusCode: 404, description: "Moto não encontrada ou sem serviços")]
    [SwaggerResponseExample(statusCode: 200, typeof(ServicoResponseListSample))]
    public async Task<IActionResult> GetByMotoId(long motoId)
    {
        var servicos = await _servicoService.ObterServicosPorMotoIdAsync(motoId);

        if (!servicos.IsSuccess) return StatusCode(servicos.StatusCode, servicos.Error);

        return Ok(servicos.Value.Select(s => new {
            s.Id,
            s.Descricao,
            s.DataCadastro,
            s.Status,
            s.MotoId,
            Colaborador = s.Colaborador != null ? new
            {
                s.Colaborador.Id,
                s.Colaborador.Nome,
                s.Colaborador.Matricula,
                s.Colaborador.Email
            } : null
        }));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra um novo serviço",
        Description = "Cadastra um novo serviço no sistema e retorna os dados cadastrados."
    )]
    [SwaggerRequestExample(typeof(ServicoDTO), typeof(ServicoRequestSample))]
    [SwaggerResponse(statusCode: 200, description: "Serviço salvo com sucesso", type: typeof(ServicoEntity))]
    [SwaggerResponseExample(statusCode: 200, typeof(ServicoResponseSample))]
    public async Task<IActionResult> Post(ServicoDTO dto)
    {
        var result = await _servicoService.AdicionarServicoAsync(dto);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza um serviço",
        Description = "Edita os dados de um serviço já cadastrado com base no ID informado."
    )]
    [SwaggerResponse(statusCode: 200, description: "Serviço atualizado com sucesso", type: typeof(ServicoEntity))]
    [SwaggerResponse(statusCode: 400, description: "Erro na requisição (validação ou dados inválidos)")]
    [SwaggerResponse(statusCode: 404, description: "Serviço não encontrado")]
    [SwaggerRequestExample(typeof(ServicoDTO), typeof(ServicoRequestSample))]
    [SwaggerResponseExample(statusCode: 200, typeof(ServicoResponseSample))]
    public async Task<IActionResult> Put(long id, ServicoDTO dto)
    {
        var result = await _servicoService.EditarServicoAsync(id, dto);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Remove um serviço",
        Description = "Exclui permanentemente um serviço com base no ID informado."
    )]
    [SwaggerResponse(statusCode: 200, description: "Serviço removido com sucesso")]
    [SwaggerResponse(statusCode: 404, description: "Serviço não encontrado")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _servicoService.DeletarServicoAsync(id);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return StatusCode(result.StatusCode, result);
    }
}
