using MiAppTerminal1.Domain;

namespace MiAppTerminal1.Api.Responses;

public sealed record TareaResponse(
    int Id,
    string Titulo,
    string Descripcion,
    string NombreResponsable,
    string ApellidoResponsable,
    bool EstaCompletado)
{
    public static TareaResponse From(Tarea tarea) => new(
        tarea.Id,
        tarea.Titulo.Valor,
        tarea.Descripcion,
        tarea.Responsable.Nombre,
        tarea.Responsable.Apellido,
        tarea.ECompletado);
}
