using System;
using System.Collections.Generic;
using System.Threading.Tasks; // <-- NUEVO USING
using MiAppTerminal1.Domain;
using MiAppTerminal1.Interfaces;

namespace MiAppTerminal1.Services;

public class TareaService
{
    private readonly ITareaRepository _repository;

    public TareaService(ITareaRepository repository)
    {
        _repository = repository;
    }

    public async Task CrearTareaAsync(string tituloRaw, string descripcion, string nombreRes, string apellidoRes)
    {
        var titulo = new TituloTarea(tituloRaw);
        var responsable = new Persona(nombreRes, apellidoRes);

        var nuevaTarea = new Tarea(0, titulo, descripcion, responsable);
        await _repository.GuardarAsync(nuevaTarea); // Espera asíncrona segura
    }

    public async Task<IEnumerable<Tarea>> ListarTareasAsync() => await _repository.ObtenerTodasAsync();

    public async Task MarcarComoTerminadaAsync(int id)
    {
        var tarea = await _repository.ObtenerPorIdAsync(id);
        if (tarea == null)
        {
            throw new KeyNotFoundException($"No se encontró la tarea con el ID: {id}");
        }

        tarea.ECompletado = true; 
        await _repository.ActualizarAsync(tarea);
    }

    public async Task ActualizarTareaAsync(int id, string nuevoTitulo, string nuevaDescripcion)
    {
        if (id <= 0) throw new ArgumentException("El ID debe ser mayor a cero.");

        var tareaExistente = await _repository.ObtenerPorIdAsync(id);
        if (tareaExistente == null)
        {
            throw new KeyNotFoundException($"No se encontró la tarea con el ID: {id}");
        }

        var tituloActualizado = new TituloTarea(nuevoTitulo);
        var tareaActualizada = new Tarea(tareaExistente.Id, tituloActualizado, nuevaDescripcion, tareaExistente.Responsable)
        {
            ECompletado = tareaExistente.ECompletado
        };

        await _repository.ActualizarAsync(tareaActualizada);
    }

    public async Task EliminarTareaAsync(int id)
    {
        if (id <= 0) throw new ArgumentException("El ID debe ser mayor a cero.");
        
        // El repositorio internamente validará si existe antes de borrar
        await _repository.EliminarAsync(id);
    }
}
