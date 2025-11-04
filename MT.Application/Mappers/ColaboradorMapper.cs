using MT.Application.Dtos;
using MT.Domain.Entities;

namespace MT.Application.Mappers;

public static class ColaboradorMapper
{
    public static ColaboradorEntity ToColaboradorEntity(this ColaboradorDTO dto)
    {
        return new ColaboradorEntity
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Matricula = dto.Matricula
        };
    }
}
