using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MiAppTerminal1.Domain;

public class Persona : IAsignable
{
    public string Nombre { get; }
    public string Apellido { get; }

    // Implementación del contrato IAsignable
    public string Identificador => $"{Apellido.ToLower()}_{Nombre.ToLower()}";
    public string NombreVisual => $"{Nombre} {Apellido}";

    [JsonConstructor]
    public Persona(string nombre, string apellido)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
        {
            throw new ArgumentException("El nombre y el apellido son obligatorios y no pueden estar vacíos.");
        }

        // Validación avanzada Clean Code utilizando expresiones regulares nativas
        if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$") || !Regex.IsMatch(apellido, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
        {
            throw new ArgumentException("El nombre y apellido solo pueden contener letras y caracteres alfabéticos comunes.");
        }

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
    }
}
