using MT.Application.Dtos;
using MT.Domain.Entities;

namespace MT.Application.Mappers;

public static class UsuarioMapper
{
    public static UsuarioEntity ToUsuarioEntity(this UsuarioDTO dto)
    {
        return new UsuarioEntity
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = dto.Senha,
        };
    }
}
