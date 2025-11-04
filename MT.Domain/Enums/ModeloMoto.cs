using System.Text.Json.Serialization;

namespace MT.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ModeloMoto
{
    MOTTU_POP,
    MOTTU_SPORT,
    MOTTU_E
}
