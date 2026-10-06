using System;
using System.Text.RegularExpressions;

namespace MiAppTerminal1.Domain;

public class Persona : IAsignable
{
    public string Nombre { get; }
    public string Apellido { get; }
    public string Identificador => $"{Apellido.ToLower()}_{Nombre.ToLower()}";
    public string NombreVisual => $"{Nombre} {Apellido}";

    public Persona(string nombre, string apellido)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
        {
            throw new ArgumentException("El nombre y el apellido son obligatorios.");
        }

        if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$") || !Regex.IsMatch(apellido, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
        {
            throw new ArgumentException("El nombre y apellido solo pueden contener letras.");
        }

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
    }
}
