using MT.Application.Dtos;
using MT.Domain.Entities;

namespace MT.Application.Mappers;

public static class MotoMapper
{
    public static MotoEntity ToMotoEntity(this MotoDTO dto)
    {
        return new MotoEntity
        {
            Placa = dto.Placa,
            Chassi = dto.Chassi,
            Modelo = dto.Modelo,
            Status = dto.Status
        };
    }
}
