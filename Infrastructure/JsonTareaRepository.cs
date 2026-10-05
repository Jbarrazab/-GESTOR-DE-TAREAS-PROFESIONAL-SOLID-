using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MiAppTerminal1.Domain;
using MiAppTerminal1.Interfaces;

namespace MiAppTerminal1.Infrastructure;

public class JsonTareaRepository : ITareaRepository
{
    private readonly string _rutaArchivo = "tareas.json";

    public void Guardar(Tarea tarea)
    {
        var tareas = ObtenerTodas().ToList();
        int idActual = tareas.Any() ? tareas.Max(t => t.Id) + 1 : 1;
        tarea.Id = idActual;
        
        tareas.Add(tarea);
        GuardarTodo(tareas);
    }

    public IEnumerable<Tarea> ObtenerTodas()
    {
        if (!File.Exists(_rutaArchivo)) return new List<Tarea>();

        try
        {
            string jsonString = File.ReadAllText(_rutaArchivo);
            return JsonSerializer.Deserialize<List<Tarea>>(jsonString) ?? new List<Tarea>();
        }
        catch (JsonException)
        {
            return new List<Tarea>();
        }
    }

    public Tarea? ObtenerPorId(int id) => ObtenerTodas().FirstOrDefault(t => t.Id == id);

    public void Actualizar(Tarea tarea)
    {
        var tareas = ObtenerTodas().ToList();
        var index = tareas.FindIndex(t => t.Id == tarea.Id);
        
        if (index != -1)
    {
        // Se elimina la línea 'tabs[index] = tarea;' que causaba el error de compilación
        tareas[index] = tarea;
        GuardarTodo(tareas);
    }
    }

    public void Eliminar(int id)
    {
        var tareas = ObtenerTodas().ToList();
        var tareaAEliminar = tareas.FirstOrDefault(t => t.Id == id);

        if (tareaAEliminar != null)
        {
            tareas.Remove(tareaAEliminar);
            GuardarTodo(tareas);
        }
        else
        {
            throw new KeyNotFoundException($"No se encontró ninguna tarea con el ID: {id} para eliminar.");
        }
    }

    private void GuardarTodo(List<Tarea> tareas)
    {
        var opciones = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(tareas, opciones);
        File.WriteAllText(_rutaArchivo, jsonString);
    }
}
