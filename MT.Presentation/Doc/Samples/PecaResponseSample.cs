using MT.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class PecaResponseSample : IExamplesProvider<PecaEntity>
{
    public PecaEntity GetExamples()
    {
        return new PecaEntity   
        {
            Id = 1,
            Nome = "Filtro de Óleo",
            Codigo = "PF456",
            Descricao = "Filtro de óleo compatível com as motos",
            QuantidadeEstoque = 30
        };
    }
}
