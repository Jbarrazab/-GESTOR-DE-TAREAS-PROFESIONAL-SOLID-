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
    public bool ECompletado { get; internal set; }
}