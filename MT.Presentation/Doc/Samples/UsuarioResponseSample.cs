using MT.Application.Dtos;
using MT.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class UsuarioResponseSample : IExamplesProvider<UsuarioResponseDTO>
{
    public UsuarioResponseDTO GetExamples()
    {
        return new UsuarioResponseDTO
        {
            Id = 1,
            Nome = "Felipe Sora",
            Email = "felipe@email.com",
            Senha = "felipe123"
        };
    }
}
