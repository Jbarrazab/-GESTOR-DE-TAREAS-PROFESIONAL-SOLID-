using System.Text.Json.Serialization;

namespace MiAppTerminal1.Infrastructure.Dtos;

public record TareaDto(
    int Id,
    string Titulo,
    string Descripcion,
    string TipoResponsable, // Nos permite saber si es "Persona", "Equipo", etc.
    string NombreResponsable,
    string ApellidoResponsable,
    bool EstaCompletado
)
{
    [JsonPropertyName("ECompletado")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ECompletadoLegacy { get; init; }
}