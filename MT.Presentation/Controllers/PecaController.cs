using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MT.Application.Dtos;
using MT.Application.Interfaces;
using MT.Domain.Entities;
using MT.Presentation.Doc.Samples;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PecaController : ControllerBase
{
    private readonly IPecaService _pecaService;

    public PecaController(IPecaService pecaService)
    {
        _pecaService = pecaService;
    }

    [HttpGet]
    [SwaggerOperation(
            Summary = "Lista de peças",
            Description = "Retorna a lista completa de peças cadastradas."
        )]
    [SwaggerResponse(statusCode: 200, description: "Lista retornada com sucesso", type: typeof(IEnumerable<PecaEntity>))]
    [SwaggerResponse(statusCode: 204, description: "Lista não tem dados")]
    [SwaggerResponseExample(statusCode: 200, typeof(PecaResponseListSample))]
    [EnableRateLimiting("MotoTrack")]
    public async Task<IActionResult> Get(int deslocamento = 0, int registrosRetornados = 10)
    {
        var result = await _pecaService.ObterTodasPecasAsync(deslocamento, registrosRetornados);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        var hateaos = new
        {
            data = result.Value.Data.Select(c => new {
                c.Id,
                c.Nome,
                c.Codigo,
                c.Descricao,
                c.QuantidadeEstoque,
                links = new
                {
                    self = Url.Action(nameof(GetId), "Peca", new { id = c.Id }, Request.Scheme),
                    put = Url.Action(nameof(Put), "Peca", new { id = c.Id }, Request.Scheme),
                    delete = Url.Action(nameof(Delete), "Peca", new { id = c.Id }, Request.Scheme)
                }
            }),
            links = new
            {
                self = Url.Action(nameof(GetId), "Peca", null),
                post = Url.Action(nameof(Post), "Peca", null, Request.Scheme),
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
            Summary = "Obtém uma peça pelo ID",
            Description = "Retorna a peça correspondente ao ID informado."
        )]
    [SwaggerResponse(statusCode: 200, description: "Peça encontrada", type: typeof(PecaEntity))]
    [SwaggerResponse(statusCode: 404, description: "Peça não encontrada")]
    [SwaggerResponseExample(statusCode: 200, typeof(PecaResponseSample))]
    public async Task<IActionResult> GetId(long id)
    {
        var result = await _pecaService.ObterPecaPorIdAsync(id);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        //return StatusCode(result.StatusCode, result);
        return Ok(result.Value);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra uma nova peça",
        Description = "Cadastra uma nova peça no sistema e retorna os dados cadastrados."
    )]
    [SwaggerRequestExample(typeof(PecaDTO), typeof(PecaRequestSample))]
    [SwaggerResponse(statusCode: 200, description: "Peça salva com sucesso", type: typeof(PecaEntity))]
    [SwaggerResponseExample(statusCode: 200, typeof(PecaResponseSample))]
    public async Task<IActionResult> Post(PecaDTO dto)
    {
        var result = await _pecaService.AdicionarPecaAsync(dto);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza uma peça",
        Description = "Edita os dados de uma peça já cadastrada com base no ID informado."
    )]
    [SwaggerResponse(statusCode: 200, description: "Peça atualizada com sucesso", type: typeof(PecaEntity))]
    [SwaggerResponse(statusCode: 400, description: "Erro na requisição (validação ou dados inválidos)")]
    [SwaggerResponse(statusCode: 404, description: "Peça não encontrada")]
    [SwaggerRequestExample(typeof(PecaDTO), typeof(PecaRequestSample))]
    [SwaggerResponseExample(statusCode: 200, typeof(PecaResponseSample))]
    public async Task<IActionResult> Put(long id, PecaDTO dto)
    {
        var result = await _pecaService.EditarPecaAsync(id, dto);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Remove uma peça",
        Description = "Exclui permanentemente uma peça com base no ID informado."
    )]
    [SwaggerResponse(statusCode: 200, description: "Peça removida com sucesso")]
    [SwaggerResponse(statusCode: 404, description: "Peça não encontrada")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _pecaService.DeletarPecaAsync(id);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return StatusCode(result.StatusCode, result);
    }
}
