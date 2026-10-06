namespace MiAppTerminal1.Domain;

public interface IAsignable
{
    string Identificador { get; }
    string NombreVisual { get; }
    string Nombre { get; }
    string Apellido { get; }
}
