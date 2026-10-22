using System.Collections.Generic;
using System.Threading.Tasks;
using MiAppTerminal1.Domain;

namespace MiAppTerminal1.Interfaces;

public interface ITareaRepository
{
    Task GuardarAsync(Tarea tarea);
    Task<IEnumerable<Tarea>> ObtenerTodasAsync();
    Task<Tarea?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <exception cref="KeyNotFoundException">The task does not exist.</exception>
    Task ActualizarAsync(Tarea tarea);

    Task EliminarAsync(int id);
}
