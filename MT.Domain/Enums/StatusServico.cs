using System.Text.Json.Serialization;

namespace MT.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusServico
{
    Pendente = 0,
    EmAndamento = 1,
    Concluido = 2,
}
