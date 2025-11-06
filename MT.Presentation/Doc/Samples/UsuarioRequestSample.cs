using MT.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class UsuarioRequestSample : IExamplesProvider<UsuarioDTO>
{
    public UsuarioDTO GetExamples()
    {
        return new UsuarioDTO("Felipe Sora", "felipe@email.com", "felipe123");
    }
}
