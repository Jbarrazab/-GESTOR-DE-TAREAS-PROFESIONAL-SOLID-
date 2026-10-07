using System.Collections.Generic;
using System.Threading.Tasks;
using MiAppTerminal1.Domain;

namespace MiAppTerminal1.Interfaces;

public interface ITareaRepository
{
    Task GuardarAsync(Tarea tarea);
    Task<IEnumerable<Tarea>> ObtenerTodasAsync();
    Task<Tarea?> ObtenerPorIdAsync(int id);
    Task ActualizarAsync(Tarea tarea);
    Task EliminarAsync(int id);
}
