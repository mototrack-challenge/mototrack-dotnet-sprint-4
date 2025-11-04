using MT.Application.Dtos;
using MT.Domain.Entities;

namespace MT.Application.Mappers;

public static class ServicoMapper
{
    public static ServicoEntity ToServicoEntity(this ServicoDTO dto)
    {
        return new ServicoEntity
        {
            Descricao = dto.Descricao,
            Status = dto.Status,
            MotoId = dto.MotoId,
            ColaboradorId = dto.ColaboradorId
        };
    }
}
