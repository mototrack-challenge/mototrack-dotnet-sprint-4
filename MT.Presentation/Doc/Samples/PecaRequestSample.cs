using MT.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class PecaRequestSample : IExamplesProvider<PecaDTO>
{
    public PecaDTO GetExamples()
    {
        return new PecaDTO("Filtro de Óleo", "PF456", "Filtro de óleo compatível com as motos", 30);
    }
}
