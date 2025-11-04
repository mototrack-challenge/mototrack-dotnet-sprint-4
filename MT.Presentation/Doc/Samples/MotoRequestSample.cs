using MT.Application.Dtos;
using MT.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class MotoRequestSample : IExamplesProvider<MotoDTO>
{
    public MotoDTO GetExamples()
    {
        return new MotoDTO
        {
            Placa = "ABC1234",
            Chassi = "9C2JC4110VR123456",
            Modelo = ModeloMoto.MOTTU_POP,
            Status = StatusMoto.DISPONIVEL
        };
    }
}
