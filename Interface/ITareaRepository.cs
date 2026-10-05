using MiAppTerminal1.Domain;
namespace MiAppTerminal1.interfaces;
public interface ItareaRepository
{
    void Guardar(Tarea tarea);
    IEnumerable<Tarea> ObtenerTodas();
    Tarea? ObtenerPorId(int id);
    void Actualizar(Tarea tarea);
}
