using MT.Application.Dtos;
using MT.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class UsuarioResponseListSample : IExamplesProvider<IEnumerable<UsuarioResponseDTO>>
{
    public IEnumerable<UsuarioResponseDTO> GetExamples()
    {
        return new List<UsuarioResponseDTO>
        {
            new UsuarioResponseDTO
            {
                Id = 1,
                Nome = "Felipe Sora",
                Email = "felipe@email.com",
                Senha = "felipe123"
            },
            new UsuarioResponseDTO
            {
                Id = 1,
                Nome = "Maria Silva",
                Email = "maria@email.com",
                Senha = "maria123"
            },
        };
    }
}
