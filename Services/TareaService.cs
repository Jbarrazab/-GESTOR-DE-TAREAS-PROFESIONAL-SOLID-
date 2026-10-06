using MiAppTerminal1.Domain;
using System.Collections.Generic;
using MiAppTerminal1.Interfaces;

namespace MiAppTerminal1.Services; // 

public class TareaService
{

    private readonly ITareaRepository _repository;

    // inyección del contrato
    public TareaService(ITareaRepository repository)
    {
        _repository = repository;
    }

    public void CrearTarea(string tituloRaw, string descripcion, string nombreResponsable, string apellidoResponsable)
    {
        var titulo = new TituloTarea(tituloRaw);
        var responsable = new Persona(nombreResponsable, apellidoResponsable);

        var nuevaTarea = new Tarea(0, titulo, descripcion, responsable);
        _repository.Guardar(nuevaTarea);
    }

    public IEnumerable<Tarea> ListarTareas()
    {
        return _repository.ObtenerTodas();
    }

    public void MarcarComoTerminada(int id)
    {
        var tarea = _repository.ObtenerPorId(id);
        if (tarea == null)
        {
            throw new KeyNotFoundException($"No se encontró ninguna tarea con el ID: {id}");
        }

        // 4. Corregido al nombre exacto de la propiedad de tu entidad Tarea
        tarea.ECompletado = true;
        _repository.Actualizar(tarea);
    }

    public void EliminiarTarea(int id)
    {
        if (id <= 0)
        {
            throw new KeyNotFoundException($"No se encontró ninguna tarea con el ID: {id}");
        }

        var tarea = _repository.ObtenerPorId(id);
        if (tarea == null)
        {
            throw new KeyNotFoundException($"No se encontró ninguna tarea con el ID: {id}");
        }

        _repository.Eliminar(id);
    }

    public void ActualizarTarea(int id, string nuevoTitulo, string nuevaDescripcion)
    {
        // 1. Validaciones defensivas del Dominio
        if (id <= 0)
        {
            throw new ArgumentException("El ID de la tarea debe ser un número entero mayor a cero.");
        }

        var tareaExistente = _repository.ObtenerPorId(id);
        if (tareaExistente == null)
        {
            throw new KeyNotFoundException($"No se encontró ninguna tarea con el ID: {id} para actualizar.");
        }

        // 2. Reconstruimos los objetos de negocio con las nuevas reglas
        var tituloActualizado = new TituloTarea(nuevoTitulo);

        // 3. Modificamos la entidad de dominio legítimamente (usando mutabilidad controlada)
        // Para cumplir Clean Code, usamos los métodos de asignación que creamos en el dominio
        var tareaActualizada = new Tarea(tareaExistente.Id, tituloActualizado, nuevaDescripcion, tareaExistente.Responsable)
        {
            ECompletado = tareaExistente.ECompletado // Mantenemos el estado de completado intacto
        };

        // 4. Mandamos la entidad modificada al repositorio (el cual la convertirá a DTO para el JSON)
        _repository.Actualizar(tareaActualizada);
    }

}
