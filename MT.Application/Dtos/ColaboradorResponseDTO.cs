namespace MT.Application.Dtos;

public class ColaboradorResponseDTO
{
    public long Id { get; set; }
    public string Nome { get; set; }
    public string Matricula { get; set; }
    public string Email { get; set; }
    public ICollection<ServicoResponseDTO> Servicos { get; set; }
}
