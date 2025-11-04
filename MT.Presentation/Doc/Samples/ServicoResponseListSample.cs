using MT.Application.Dtos;
using MT.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace MT.Presentation.Doc.Samples;

public class ServicoResponseListSample : IExamplesProvider<IEnumerable<ServicoResponseDTO>>
{
    public IEnumerable<ServicoResponseDTO> GetExamples()
    {
        return new List<ServicoResponseDTO>
        {
            new ServicoResponseDTO
            {
                Id = 1,
                Descricao = "Troca de óleo",
                DataCadastro = DateTime.Now,
                Status = StatusServico.EmAndamento,
                MotoId = 2,
                Colaborador = new ColaboradorResponseDTO
                {
                    Id = 1,
                    Nome = "Felipe Sora",
                    Matricula = "620184901",
                    Email = "felipe@email.com"
                }
            },
            new ServicoResponseDTO
            {
                Id = 2,
                Descricao = "Revisão do motor",
                DataCadastro = DateTime.Now,
                Status = StatusServico.Pendente,
                MotoId = 3,
                Colaborador = new ColaboradorResponseDTO
                {
                    Id = 1,
                    Nome = "Felipe Sora",
                    Matricula = "620184901",
                    Email = "felipe@email.com"
                }
            }
        };
    }
}