
using System;
using System.Text.Json.Serialization; 
namespace MiAppTerminal1.Domain;

public class Tarea
{
    public int Id { get; set; }
    public TituloTarea Titulo {get; private set;}
    public string Descripcion { get; set; } =string.Empty;
    public IAsignable Responsable {get; private set;} // Pol en la asignación
    public bool ECompletado { get; set; }


    [JsonConstructor]
    public Tarea(int id, TituloTarea titulo, string descripcion, IAsignable responsable)  
    {
        Id = id;
        Titulo = titulo;
        Descripcion = descripcion;
        Responsable = responsable;
        ECompletado = false;
    }

}
