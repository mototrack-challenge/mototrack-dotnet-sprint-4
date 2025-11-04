using MT.Application.Dtos;
using MT.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class ServicoRequestSample : IExamplesProvider<ServicoDTO>
{
    public ServicoDTO GetExamples()
    {
        return new ServicoDTO(
            Descricao: "Troca de óleo",
            Status: StatusServico.Pendente,
            MotoId: 2,
            ColaboradorId: 1
        );
    }
}
