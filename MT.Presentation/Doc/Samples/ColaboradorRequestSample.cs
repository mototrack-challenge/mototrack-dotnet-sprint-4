using MT.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class ColaboradorRequestSample : IExamplesProvider<ColaboradorDTO>
{
    public ColaboradorDTO GetExamples()
    {
        return new ColaboradorDTO("Felipe Sora", "620184901", "felipe@email.com");
    }
}
