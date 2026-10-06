using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MiAppTerminal1.Domain;
using MiAppTerminal1.Interfaces;
using MiAppTerminal1.Infrastructure.Dtos;

namespace MiAppTerminal1.Infrastructure;

public class JsonTareaRepository : ITareaRepository
{
    private readonly string _rutaArchivo;

    public JsonTareaRepository(IConfiguration configuration)
    {
        _rutaArchivo = configuration["ConfiguracionTareas:RutaArchivo"] ?? "tareas.json";
    }

    public void Guardar(Tarea tarea)
    {
        var dtos = LeerDtosDesdeDisco();
        int nuevoId = dtos.Any() ? dtos.Max(d => d.Id) + 1 : 1;
        tarea.Id = nuevoId;

        // Transformación: Dominio -> DTO
        var nuevoDto = MapearADto(tarea);
        dtos.Add(nuevoDto);
        
        GuardarDtosEnDisco(dtos);
    }

    public IEnumerable<Tarea> ObtenerTodas()
    {
        // Transformación: DTO -> Dominio
        return LeerDtosDesdeDisco().Select(MapearADominio);
    }

    public Tarea? ObtenerPorId(int id)
    {
        var dto = LeerDtosDesdeDisco().FirstOrDefault(d => d.Id == id);
        return dto != null ? MapearADominio(dto) : null;
    }

    public void Actualizar(Tarea tarea)
    {
        var dtos = LeerDtosDesdeDisco();
        var index = dtos.FindIndex(d => d.Id == tarea.Id);

        if (index != -1)
        {
            dtos[index] = MapearADto(tarea);
            GuardarDtosEnDisco(dtos);
        }
    }

    public void Eliminar(int id)
    {
        var dtos = LeerDtosDesdeDisco();
        var dtoAEliminar = dtos.FirstOrDefault(d => d.Id == id);

        if (dtoAEliminar != null)
        {
            dtos.Remove(dtoAEliminar);
            GuardarTodo(dtos);
        }
        else
        {
            throw new KeyNotFoundException($"No se encontró ninguna tarea con el ID: {id} para eliminar.");
        }
    }

    // ==========================================
    // MÉTODOS DE MAPEO (TRANSFORMACIÓN DE CAPAS)
    // ==========================================
    
    private TareaDto MapearADto(Tarea tarea)
    {
        return new TareaDto(
            tarea.Id,
            tarea.Titulo.Valor,
            tarea.Descripcion,
            tarea.Responsable.GetType().Name.ToLower(), // Guarda "persona"
            tarea.Responsable.Nombre,
            tarea.Responsable.Apellido,
            tarea.ECompletado
        );
    }

    private Tarea MapearADominio(TareaDto dto)
    {
        IAsignable responsable = dto.TipoResponsable switch
        {
            "persona" => new Persona(dto.NombreResponsable, dto.ApellidoResponsable),
            _ => throw new NotSupportedException($"Tipo de responsable '{dto.TipoResponsable}' no soportado.")
        };

        var tarea = new Tarea(dto.Id, new TituloTarea(dto.Titulo), dto.Descripcion, responsable)
        {
            ECompletado = dto.EstaCompletado
        };
        return tarea;
    }

    // ==========================================
    // MÉTODOS DE INFRAESTRUCTURA DE BAJO NIVEL
    // ==========================================

    private List<TareaDto> LeerDtosDesdeDisco()
    {
        if (!File.Exists(_rutaArchivo)) return new List<TareaDto>();
        try
        {
            string jsonString = File.ReadAllText(_rutaArchivo);
            return JsonSerializer.Deserialize<List<TareaDto>>(jsonString) ?? new List<TareaDto>();
        }
        catch (JsonException) { return new List<TareaDto>(); }
    }

    private void GuardarDtosEnDisco(List<TareaDto> dtos) => GuardarTodo(dtos);

    private void GuardarTodo(List<TareaDto> dtos)
    {
        var opciones = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(dtos, opciones);
        File.WriteAllText(_rutaArchivo, jsonString);
    }
}
