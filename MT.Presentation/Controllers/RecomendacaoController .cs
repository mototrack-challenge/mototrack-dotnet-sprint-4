using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;
using MT.Application.Interfaces;
using MT.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace MT.Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RecomendacaoController : ControllerBase
{
    private readonly MLContext mLContext;
    private readonly string caminhoModelo = Path.Combine(Environment.CurrentDirectory, "Treinamento", "ModeloRecomendacaoServico.zip");
    private readonly IServicoService _servicoService;

    public RecomendacaoController(IServicoService servicoService)
    {
        mLContext = new MLContext(seed: 0);
        _servicoService = servicoService;
    }

    // Classe de dados para treino (IDs como uint para Matrix Factorization)
    public class DadosRecomendacaoServico
    {
        [LoadColumn(0)] public uint ColaboradorId;
        [LoadColumn(1)] public uint ServicoId;
        [LoadColumn(2)] public float Nota;
    }

    // Classe de resultado da predição
    public class RecomendacaoServico
    {
        [ColumnName("Score")]
        public float PontuacaoRecomendacao;
    }

    // Treinar modelo usando Matrix Factorization
    [HttpGet("Treinar")]
    [SwaggerOperation(
        Summary = "Treina o modelo de recomendação de serviços",
        Description = "Treina o modelo ML.NET usando histórico de serviços e colaboradores, salvando o modelo em disco."
    )]
    [SwaggerResponse(200, "Modelo treinado com sucesso.")]
    [SwaggerResponse(400, "Erro ao obter dados de serviços para treinamento.")]
    public async Task<IActionResult> Treinar()
    {
        var result = await _servicoService.ObterTodosServicosAsync();
        if (!result.IsSuccess) return BadRequest(result.Error);

        var servicos = result.Value.Data;

        // Mapear IDs para uint (Matrix Factorization precisa de IDs inteiros)
        var dadosContext = servicos.Select(x => new DadosRecomendacaoServico
        {
            ColaboradorId = (uint)x.ColaboradorId,
            ServicoId = (uint)x.Id,
            Nota = x.Status == Domain.Enums.StatusServico.Concluido ? 5f : 1f
        });

        IDataView dadosTreinados = mLContext.Data.LoadFromEnumerable(dadosContext);

        var pipeline = mLContext.Transforms
        .Conversion.MapValueToKey(outputColumnName: "ColaboradorIdKey", inputColumnName: nameof(DadosRecomendacaoServico.ColaboradorId))
        .Append(mLContext.Transforms.Conversion.MapValueToKey(outputColumnName: "ServicoIdKey", inputColumnName: nameof(DadosRecomendacaoServico.ServicoId)))
        .Append(mLContext.Recommendation().Trainers.MatrixFactorization(
            labelColumnName: nameof(DadosRecomendacaoServico.Nota),
            matrixColumnIndexColumnName: "ColaboradorIdKey",
            matrixRowIndexColumnName: "ServicoIdKey",
            numberOfIterations: 20,
            approximationRank: 100
        ));

        var modelo = pipeline.Fit(dadosTreinados);
        mLContext.Model.Save(modelo, dadosTreinados.Schema, caminhoModelo);

        return Ok(new { data = "Modelo treinado com sucesso!" });
    }

    // Fazer recomendações
    [HttpPost("{colaboradorId}")]
    [SwaggerOperation(
        Summary = "Gera recomendações de serviços para um colaborador",
        Description = "Recebe o colaboradorId e uma lista de IDs de serviços, retornando uma pontuação de recomendação para cada serviço."
    )]
    [SwaggerResponse(200, "Recomendações geradas com sucesso.")]
    [SwaggerResponse(400, "Modelo de recomendação não foi treinado ou parâmetros inválidos.")]
    public async Task<IActionResult> RecomendarServico(uint colaboradorId, [FromBody] IEnumerable<uint> servicosIds)
    {
        if (!System.IO.File.Exists(caminhoModelo))
            return BadRequest("O modelo não foi treinado!");

        ITransformer modelo;
        using var stream = new FileStream(caminhoModelo, FileMode.Open, FileAccess.Read, FileShare.Read);
        modelo = mLContext.Model.Load(stream, out var modeloSchema);

        var engine = mLContext.Model.CreatePredictionEngine<DadosRecomendacaoServico, RecomendacaoServico>(modelo);
        var listRetorno = new List<dynamic>();

        foreach (var servicoId in servicosIds)
        {
            var recomendacao = engine.Predict(new DadosRecomendacaoServico
            {
                ColaboradorId = colaboradorId,
                ServicoId = servicoId
            });

            var servicoResult = await _servicoService.ObterServicoPorIdAsync(servicoId);
            var servico = servicoResult.IsSuccess ? servicoResult.Value : null;
            var score = recomendacao.PontuacaoRecomendacao;

            // Substituir valores inválidos por 0 ou outro padrão
            if (float.IsNaN(score) || float.IsInfinity(score))
                score = 0f;

            listRetorno.Add(new
            {
                Servico = servico?.Descricao ?? string.Empty,
                Status = servico?.Status.ToString() ?? string.Empty,
                Score = score,
                Recomendado = ClassificarRecomendacao(score)
            });
        }

        return Ok(new
        {
            data = listRetorno,
            status = StatusCodes.Status200OK
        });
    }

    private string ClassificarRecomendacao(double pontuacao)
    {
        return pontuacao >= 4 ? "Altamente Recomendado"
             : pontuacao >= 3 ? "Recomendado"
             : "Não Recomendado";
    }
}
