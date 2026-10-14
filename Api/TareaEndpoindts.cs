using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MiAppTerminal1.Api.Requests;
using MiAppTerminal1.Api.Responses;
using MiAppTerminal1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace MiAppTerminal1.Api;

public static class TareaEndpoints
{
    public static void MapTareaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tareas")
            .WithTags("Tareas");

        group.MapGet("/", ListarAsync)
            .WithName("ListarTareas")
            .WithSummary("Lista las tareas")
            .WithDescription("Devuelve todas las tareas con los datos públicos de cada responsable.")
            .Produces<IReadOnlyList<TareaResponse>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}", ObtenerPorIdAsync)
            .WithName("ObtenerTareaPorId")
            .WithSummary("Obtiene una tarea por su ID")
            .WithDescription("Devuelve la tarea identificada por el ID.")
            .Produces<TareaResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapPost("/", CrearAsync)
            .WithName("CrearTarea")
            .WithSummary("Crea una tarea")
            .WithDescription("Crea una tarea y devuelve la tarea creada.")
            .Produces<TareaResponse>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapPut("/completar/{id:int}", CompletarAsync)
            .WithName("CompletarTarea")
            .WithSummary("Marca una tarea como completada")
            .WithDescription("Marca como completada la tarea identificada por el ID.")
            .Produces<MensajeResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}", ActualizarAsync)
            .WithName("ActualizarTarea")
            .WithSummary("Actualiza una tarea")
            .WithDescription("Actualiza el título y la descripción de una tarea existente.")
            .Produces<MensajeResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:int}", EliminarAsync)
            .WithName("EliminarTarea")
            .WithSummary("Elimina una tarea")
            .WithDescription("Elimina la tarea identificada por el ID.")
            .Produces<MensajeResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<Ok<IReadOnlyList<TareaResponse>>> ListarAsync(TareaService servicio)
    {
        var tareas = await servicio.ListarTareasAsync();
        IReadOnlyList<TareaResponse> response = tareas.Select(TareaResponse.From).ToArray();
        return TypedResults.Ok(response);
    }

    private static async Task<IResult> ObtenerPorIdAsync(
        [Range(1, int.MaxValue)] int id,
        HttpContext context,
        TareaService servicio)
    {
        try
        {
            var tarea = await servicio.ObtenerTareaPorIdAsync(id);
            return TypedResults.Ok(TareaResponse.From(tarea));
        }
        catch (System.ArgumentException)
        {
            return CrearError(context, StatusCodes.Status400BadRequest);
        }
        catch (KeyNotFoundException)
        {
            return CrearError(context, StatusCodes.Status404NotFound);
        }
    }

    private static async Task<IResult> CrearAsync(
        CrearTareaRequest request,
        HttpContext context,
        TareaService servicio)
    {
        try
        {
            var tarea = await servicio.CrearTareaAsync(
                request.Titulo,
                request.Descripcion,
                request.NombreResponsable,
                request.ApellidoResponsable);

            return TypedResults.Created($"/api/tareas/{tarea.Id}", TareaResponse.From(tarea));
        }
        catch (System.ArgumentException)
        {
            return CrearError(context, StatusCodes.Status400BadRequest);
        }
    }

    private static async Task<IResult> CompletarAsync(
        [Range(1, int.MaxValue)] int id,
        HttpContext context,
        TareaService servicio)
    {
        try
        {
            await servicio.MarcarComoTerminadaAsync(id);
            return TypedResults.Ok(new MensajeResponse("Tarea marcada como completada."));
        }
        catch (System.ArgumentException)
        {
            return CrearError(context, StatusCodes.Status400BadRequest);
        }
        catch (KeyNotFoundException)
        {
            return CrearError(context, StatusCodes.Status404NotFound);
        }
    }

    private static async Task<IResult> ActualizarAsync(
        [Range(1, int.MaxValue)] int id,
        ActualizarTareaRequest request,
        HttpContext context,
        TareaService servicio)
    {
        try
        {
            await servicio.ActualizarTareaAsync(id, request.NuevoTitulo, request.NuevaDescripcion);
            return TypedResults.Ok(new MensajeResponse("Tarea actualizada de forma asíncrona."));
        }
        catch (System.ArgumentException)
        {
            return CrearError(context, StatusCodes.Status400BadRequest);
        }
        catch (KeyNotFoundException)
        {
            return CrearError(context, StatusCodes.Status404NotFound);
        }
    }

    private static async Task<IResult> EliminarAsync(
        [Range(1, int.MaxValue)] int id,
        HttpContext context,
        TareaService servicio)
    {
        try
        {
            await servicio.EliminarTareaAsync(id);
            return TypedResults.Ok(new MensajeResponse("Tarea eliminada de la API de forma segura."));
        }
        catch (System.ArgumentException)
        {
            return CrearError(context, StatusCodes.Status400BadRequest);
        }
        catch (KeyNotFoundException)
        {
            return CrearError(context, StatusCodes.Status404NotFound);
        }
    }

    private static IResult CrearError(HttpContext context, int statusCode) =>
        Results.Json(
            new ApiErrorResponse(statusCode, context.TraceIdentifier),
            statusCode: statusCode);
}
