using System.ComponentModel.DataAnnotations;

namespace MiAppTerminal1.Api.Requests;

public record CrearTareaRequest(
    [property: RegularExpression(@"\S")]
    string Titulo, 
    string Descripcion, 
    [property: RegularExpression(@"^(?=.*\S)[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")]
    string NombreResponsable, 
    [property: RegularExpression(@"^(?=.*\S)[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")]
    string ApellidoResponsable
);