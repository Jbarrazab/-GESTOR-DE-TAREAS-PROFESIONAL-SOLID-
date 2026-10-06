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

    public void EliminiarTarea (int id)
    {
        if (id <=0)
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

}
