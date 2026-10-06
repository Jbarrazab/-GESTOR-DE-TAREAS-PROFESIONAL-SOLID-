using System.Collections.Generic;
using MiAppTerminal1.Domain;

namespace MiAppTerminal1.Interfaces;

public interface ITareaRepository
{
    void Guardar(Tarea tarea);
    IEnumerable<Tarea> ObtenerTodas();
    Tarea? ObtenerPorId(int id);
    void Actualizar(Tarea tarea);
    void Eliminar(int id);
}
