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
        Valor = valor.Trim();
    }
}
