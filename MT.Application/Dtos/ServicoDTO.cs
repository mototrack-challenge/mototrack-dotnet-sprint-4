using MT.Domain.Enums;

namespace MT.Application.Dtos;

public record ServicoDTO(string Descricao, StatusServico Status, long MotoId, long ColaboradorId);
