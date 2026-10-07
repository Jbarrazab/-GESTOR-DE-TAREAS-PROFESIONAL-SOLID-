using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiAppTerminal1.Domain;
using MiAppTerminal1.Interfaces;

namespace MiAppTerminal1.Infrastructure;

public class MemoriaTareaRepository : ITareaRepository
{
    private readonly List<Tarea> _tareas = new();

    public Task GuardarAsync(Tarea tarea)
    {
        int nuevoId = _tareas.Any() ? _tareas.Max(t => t.Id) + 1 : 1;
        tarea.Id = nuevoId;
        _tareas.Add(tarea);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Tarea>> ObtenerTodasAsync()
    {
        return Task.FromResult<IEnumerable<Tarea>>(_tareas);
    }

    public Task<Tarea?> ObtenerPorIdAsync(int id)
    {
        var tarea = _tareas.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(tarea);
    }

    public Task ActualizarAsync(Tarea tarea)
    {
        var index = _tareas.FindIndex(t => t.Id == tarea.Id);
        if (index != -1)
        {
            _tareas[index] = tarea;
        }
        return Task.CompletedTask;
    }

    public Task EliminarAsync(int id)
    {
        var tarea = _tareas.FirstOrDefault(t => t.Id == id);
        if (tarea != null)
        {
            _tareas.Remove(tarea);
        }
        return Task.CompletedTask;
    }
}
