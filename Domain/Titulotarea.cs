using System;

namespace MiAppTerminal1.Domain;

public class TituloTarea
{
    public string Valor { get; }

    public TituloTarea(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException("El título de la tarea no puede estar vacío.");
        }

        if (valor.Length < 3 || valor.Length > 80)
        {
            throw new ArgumentException("El título debe tener entre 3 y 100 caracteres por restricciones de negocio.");
        }

        Valor = valor.Trim();
    }

    // Conversión implícita para poder usar el objeto como un string común cuando sea necesario
    public static implicit operator string(TituloTarea titulo) => titulo.Valor;
}
