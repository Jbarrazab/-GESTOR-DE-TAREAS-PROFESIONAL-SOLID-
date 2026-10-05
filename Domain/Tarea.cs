
using System;
namespace MiAppTerminal1.Domain;

public class Tarea
{
    public int Id { get; set; }
    public string Descripcion { get; set; } =string.Empty;
    public bool ECompletado { get; set; }
    public Tarea(int id, string descripcion)
    {
        Id = id;
        Descripcion = descripcion;
        ECompletado = false;
    }

}
