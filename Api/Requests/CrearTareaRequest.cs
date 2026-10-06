namespace MiAppTerminal1.Api.Requests;

public record CrearTareaRequest(
    string Titulo, 
    string Descripcion, 
    string NombreResponsable, 
    string ApellidoResponsable
);
