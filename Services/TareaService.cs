using System;
using MiAppTerminal1.Domain;
using MiAppTerminal1.interfaces; // 1. Corregido a mayúscula para coincidir con tu carpeta

namespace MiAppTerminal1.Services; // 2. Corregido a plural para alinearse con las buenas prácticas

public class TareaService
{
    // 3. Corregido el contrato a ITareaRepository (con T mayúscula)
    private readonly ItareaRepository _repository;

    // Aquí hacemos la inyección del contrato
    public TareaService(ItareaRepository repository)
    {
        _repository = repository;
    }

    public void CrearTarea(string descripcion)
    {
        // Validar que no se inserten espacios en blanco
        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException("La descripción de la tarea no puede estar vacía.");   
        }

        var nuevaTarea = new Tarea(0, descripcion);
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
}
