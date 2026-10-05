using System.Collections.Generic;
using System.Linq;
using MiAppTerminal1.Domain;
using MiAppTerminal1.Interfaces;

namespace MiAppTerminal1.Infrastructure;

public class MemoriaTareaRepository : ITareaRepository
{
    private readonly List<Tarea> _tareas = new();
    private int _idActual = 1;

    public void Guardar(Tarea tarea)
    {
        tarea.Id = _idActual++;
        _tareas.Add(tarea);
    }

    public IEnumerable<Tarea> ObtenerTodas() => _tareas;

    public Tarea? ObtenerPorId(int id) => _tareas.FirstOrDefault(t => t.Id == id);

    public void Actualizar(Tarea tarea)
    {
        var index = _tareas.FindIndex(t => t.Id == tarea.Id);
        if (index != -1)
        {
            _tareas[index] = tarea;
        }
    }

    public void Eliminar(int id)
    {
        var tareaAEliminar = _tareas.FirstOrDefault(t => t.Id == id);
        if (tareaAEliminar != null)
        {
            _tareas.Remove(tareaAEliminar);
        }
        else
        {
            throw new KeyNotFoundException($"No se encontró ninguna tarea con el ID: {id} para eliminar.");
        }
    }
}
