using MT.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class AutenticacaoRequestSample : IExamplesProvider<AutenticacaoDTO>
{
    public AutenticacaoDTO GetExamples()
    {
        return new AutenticacaoDTO("felipe@email.com", "felipe123");
    }
}
