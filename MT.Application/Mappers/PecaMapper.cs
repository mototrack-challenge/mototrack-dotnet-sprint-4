using MT.Application.Dtos;
using MT.Domain.Entities;

namespace MT.Application.Mappers;

public static class PecaMapper
{
    public static PecaEntity ToPecaEntity(this PecaDTO dto)
    {
        return new PecaEntity
        {
            Nome = dto.Nome,
            Codigo = dto.Codigo,
            Descricao = dto.Descricao,
            QuantidadeEstoque = dto.QuantidadeEstoque
        };
    }
}
