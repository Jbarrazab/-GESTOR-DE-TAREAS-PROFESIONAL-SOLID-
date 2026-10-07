using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiAppTerminal1.Services;
using MiAppTerminal1.Api.Requests; // 💡 Importamos la nueva carpeta arquitectónica

namespace MiAppTerminal1.Api;

public static class TareaEndpoints
{
    public static void MapTareaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tareas");

        // GET: /api/tareas
        group.MapGet("/", async (TareaService servicio) =>
        {
            var tareas = await servicio.ListarTareasAsync();
            return Results.Ok(tareas);
        });

        // POST: /api/tareas
        group.MapPost("/", async (CrearTareaRequest request, TareaService servicio) =>
        {
            try
            {
                await servicio.CrearTareaAsync(
                    request.Titulo, 
                    request.Descripcion, 
                    request.NombreResponsable, 
                    request.ApellidoResponsable
                );
                return Results.Created($"/api/tareas", "¡Tarea creada asíncronamente en la API!");
            }
            catch (System.ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT: /api/tareas/completar/{id}
        group.MapPut("/completar/{id:int}", async (int id, TareaService servicio) =>
        {
            try
            {
                await servicio.MarcarComoTerminadaAsync(id);
                return Results.Ok(new { mensaje = "Tarea marcada como completada." });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
        });

        // PUT: /api/tareas/{id}
        group.MapPut("/{id:int}", async (int id, ActualizarTareaRequest request, TareaService servicio) =>
        {
            try
            {
                await servicio.ActualizarTareaAsync(id, request.NuevoTitulo, request.NuevaDescripcion);
                return Results.Ok(new { mensaje = "Tarea actualizada de forma asíncrona." });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (System.ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // DELETE: /api/tareas/{id}
        group.MapDelete("/{id:int}", async (int id, TareaService servicio) =>
        {
            try
            {
                await servicio.EliminarTareaAsync(id);
                return Results.Ok(new { mensaje = "Tarea eliminada de la API de forma segura." });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (System.ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }
}
