using MT.Domain.Enums;

namespace MT.Application.Dtos;

public class MotoResponseDTO
{
    public long Id { get; set; }
    public string Placa { get; set; }
    public string Chassi { get; set; }
    public ModeloMoto Modelo { get; set; }
    public StatusMoto Status { get; set; }
    public ICollection<ServicoResponseDTO> Servicos { get; set; }
}
