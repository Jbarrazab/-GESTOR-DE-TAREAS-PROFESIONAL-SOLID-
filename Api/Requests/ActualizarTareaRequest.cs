using System.ComponentModel.DataAnnotations;

namespace MiAppTerminal1.Api.Requests;

public record ActualizarTareaRequest(
    [property: RegularExpression(@"\S")]
    string NuevoTitulo, 
    string NuevaDescripcion
);
