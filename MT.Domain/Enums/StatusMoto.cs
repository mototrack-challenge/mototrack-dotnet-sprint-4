using System.Text.Json.Serialization;

namespace MT.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusMoto
{
    MANUTENCAO,
    DISPONIVEL,
    AVALIACAO
}
