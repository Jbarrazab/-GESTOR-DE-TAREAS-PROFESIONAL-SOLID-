using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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

    public async Task GuardarAsync(Tarea tarea)
    {
        var dtos = await LeerDtosDesdeDiscoAsync();
        int nuevoId = dtos.Any() ? dtos.Max(d => d.Id) + 1 : 1;
        tarea.Id = nuevoId;

        var nuevoDto = MapearADto(tarea);
        dtos.Add(nuevoDto);
        
        await GuardarDtosEnDiscoAsync(dtos);
    }

    public async Task<IEnumerable<Tarea>> ObtenerTodasAsync()
    {
        var dtos = await LeerDtosDesdeDiscoAsync();
        return dtos.Select(MapearADominio);
    }

    public async Task<Tarea?> ObtenerPorIdAsync(int id)
    {
        var dtos = await LeerDtosDesdeDiscoAsync();
        var dto = dtos.FirstOrDefault(d => d.Id == id);
        return dto != null ? MapearADominio(dto) : null;
    }

    public async Task ActualizarAsync(Tarea tarea)
    {
        var dtos = await LeerDtosDesdeDiscoAsync();
        var index = dtos.FindIndex(d => d.Id == tarea.Id);

        if (index != -1)
        {
            dtos[index] = MapearADto(tarea);
            await GuardarDtosEnDiscoAsync(dtos);
        }
    }

    public async Task EliminarAsync(int id)
    {
        var dtos = await LeerDtosDesdeDiscoAsync();
        var dtoAEliminar = dtos.FirstOrDefault(d => d.Id == id);

        if (dtoAEliminar != null)
        {
            dtos.Remove(dtoAEliminar);
            await GuardarDtosEnDiscoAsync(dtos);
        }
        else
        {
            throw new KeyNotFoundException($"No se encontró ninguna tarea con el ID: {id} para eliminar.");
        }
    }

    // ==========================================
    // MÉTODO DE LECTURA/ESCRITURA ASÍNCRONA REAL
    // ==========================================

    private async Task<List<TareaDto>> LeerDtosDesdeDiscoAsync()
    {
        if (!File.Exists(_rutaArchivo)) return new List<TareaDto>();
        try
        {
            // 💡 No bloquea el hilo principal del servidor mientras lee del disco
            string jsonString = await File.ReadAllTextAsync(_rutaArchivo);
            return JsonSerializer.Deserialize<List<TareaDto>>(jsonString) ?? new List<TareaDto>();
        }
        catch (JsonException) { return new List<TareaDto>(); }
    }

    private async Task GuardarDtosEnDiscoAsync(List<TareaDto> dtos)
    {
        var opciones = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(dtos, opciones);
        // 💡 No bloquea al servidor mientras escribe los bytes físicamente
        await File.WriteAllTextAsync(_rutaArchivo, jsonString);
    }

    private TareaDto MapearADto(Tarea tarea) => new(
        tarea.Id, tarea.Titulo.Valor, tarea.Descripcion, 
        tarea.Responsable.GetType().Name.ToLower(), 
        tarea.Responsable.Nombre, tarea.Responsable.Apellido, tarea.ECompletado
    );

    private Tarea MapearADominio(TareaDto dto)
    {
        IAsignable responsable = dto.TipoResponsable switch
        {
            "persona" => new Persona(dto.NombreResponsable, dto.ApellidoResponsable),
            _ => throw new NotSupportedException($"Tipo '{dto.TipoResponsable}' no soportado.")
        };

        return new Tarea(dto.Id, new TituloTarea(dto.Titulo), dto.Descripcion, responsable)
        {
            ECompletado = dto.ECompletado
        };
    }
}
