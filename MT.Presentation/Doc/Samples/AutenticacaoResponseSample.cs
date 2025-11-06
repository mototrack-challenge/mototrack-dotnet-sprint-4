using MT.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class AutenticacaoResponseSample : IExamplesProvider<AutenticacaoResponseDTO>
{
    public AutenticacaoResponseDTO GetExamples()
    {
        return new AutenticacaoResponseDTO
        {
            User = "felipe@email.com",
            Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImZlbGlwZUBlbWFpbC5jb20iLCJuYmYiOjE3NjIzOTc5NzYsImV4cCI6MTc2MjQyNjc3NiwiaWF0IjoxNzYyMzk3OTc2fQ.3AWI46mkeEm7XZpQvSV3bXqkYSSeA52UUAZNwBsGAdg"
        };
    }
}
