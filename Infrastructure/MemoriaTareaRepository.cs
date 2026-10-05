using System.Data.Common;
using MiAppTerminal1.Domain;
using MiAppTerminal1.interfaces;

namespace MiAppTerminal1.Infrastructure;

public class MemoriaTareaRepository : ItareaRepository
{
    //Simula muestra de tabla de la base de datos en la memorisa R
    private readonly List<Tarea> _tareas =new();
    private int _idActual = 1;
    public void Guardar (Tarea tarea)
    {
        tarea.Id = _idActual++;
        _tareas.Add(tarea);
    }
    public IEnumerable<Tarea> ObtenerTodas()
    {
        return _tareas;
    }
    public Tarea? ObtenerPorId (int id)
    {
        return _tareas.FirstOrDefault(t => t.Id == id);
    
    }
    public void Actualizar (Tarea tarea)
    {
        var index = _tareas.FindIndex(t => t.Id == tarea.Id);
        if (index != -1)
        {
            _tareas[index] = tarea;
        }
    }

}   
