using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MiAppTerminal1.Domain;
using MiAppTerminal1.Interfaces;
using MiAppTerminal1.Infrastructure.Dtos;

namespace MiAppTerminal1.Infrastructure;

public class JsonTareaRepository : ITareaRepository
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> FileLocks = new(
        OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

    private readonly string _rutaArchivo;
    private readonly SemaphoreSlim _fileLock;

    public JsonTareaRepository(IConfiguration configuration)
    {
        _rutaArchivo = Path.GetFullPath(configuration["ConfiguracionTareas:RutaArchivo"] ?? "tareas.json");
        _fileLock = FileLocks.GetOrAdd(_rutaArchivo, _ => new SemaphoreSlim(1, 1));
    }

    public async Task GuardarAsync(Tarea tarea)
    {
        await _fileLock.WaitAsync();
        try
        {
            var dtos = await LeerDtosDesdeDiscoAsync();
            int nuevoId = dtos.Any() ? dtos.Max(d => d.Id) + 1 : 1;
            tarea.AsignarId(nuevoId);

            var nuevoDto = MapearADto(tarea);
            dtos.Add(nuevoDto);

            await GuardarDtosEnDiscoAsync(dtos);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task<IEnumerable<Tarea>> ObtenerTodasAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            var dtos = await LeerDtosDesdeDiscoAsync();
            return dtos.Select(MapearADominio).ToArray();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task<Tarea?> ObtenerPorIdAsync(int id)
    {
        await _fileLock.WaitAsync();
        try
        {
            var dtos = await LeerDtosDesdeDiscoAsync();
            var dto = dtos.FirstOrDefault(d => d.Id == id);
            return dto != null ? MapearADominio(dto) : null;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task ActualizarAsync(Tarea tarea)
    {
        await _fileLock.WaitAsync();
        try
        {
            var dtos = await LeerDtosDesdeDiscoAsync();
            var index = dtos.FindIndex(d => d.Id == tarea.Id);

            if (index != -1)
            {
                dtos[index] = MapearADto(tarea);
                await GuardarDtosEnDiscoAsync(dtos);
                return;
            }

            throw new KeyNotFoundException($"No se encontró ninguna tarea con el ID: {tarea.Id} para actualizar.");
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task EliminarAsync(int id)
    {
        await _fileLock.WaitAsync();
        try
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
        finally
        {
            _fileLock.Release();
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
            return JsonSerializer.Deserialize<List<TareaDto>>(jsonString)
                ?? throw new InvalidDataException("El archivo de tareas no contiene una colección JSON válida.");
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException("El archivo de tareas contiene JSON inválido.", ex);
        }
    }

    private async Task GuardarDtosEnDiscoAsync(List<TareaDto> dtos)
    {
        var opciones = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(dtos, opciones);
        string temporaryPath = $"{_rutaArchivo}.{Guid.NewGuid():N}.tmp";
        try
        {
            await File.WriteAllTextAsync(temporaryPath, jsonString);
            File.Move(temporaryPath, _rutaArchivo, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
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

        return new Tarea(
            dto.Id,
            new TituloTarea(dto.Titulo),
            dto.Descripcion,
            responsable,
            dto.EstaCompletado || dto.ECompletadoLegacy == true);
    }
}
