using System;
using MiAppTerminal1.Services;

namespace MiAppTerminal1.Ui;

public class ConsoleUi
{
    // Línea 8 corregida: Ahora este campo privado vive dentro de la clase
    private readonly TareaService _servicio;

    // Línea 11 corregida: El constructor ahora pertenece legítimamente a la clase
    public ConsoleUi(TareaService servicio)
    {
        _servicio = servicio;
    }

    public void EjecutarMenu()
    {
        bool continuar = true;

        while (continuar)
        {
            Console.Clear();
            Console.WriteLine("=== GESTOR DE TAREAS PROFESIONAL (SOLID) ===");
            Console.WriteLine("1. Agregar Tarea");
            Console.WriteLine("2. Listar Tareas");
            Console.WriteLine("3. Marcar Tarea como Completada");
            Console.WriteLine("4. Eliminar Tarea");
            Console.WriteLine("5. Salir");
            Console.Write("Selecciona una opción: ");

            string? opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.Write("\nEscribe la descripción de la tarea: ");
                    string? desc = Console.ReadLine();
                    try
                    {
                        _servicio.CrearTarea(desc ?? string.Empty);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("¡Tarea agregada con éxito!");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error de validación: {ex.Message}");
                    }
                    finally { Console.ResetColor(); }
                    break;

                case "2":
                    Console.WriteLine("\n--- LISTA DE TAREAS ---");
                    var tareas = _servicio.ListarTareas();
                    if (!tareas.Any())
                    {
                        Console.WriteLine("No hay tareas registradas aún.");
                    }
                    else
                    {
                        foreach (var t in tareas)
                        {
                            string estado = t.ECompletado ? "[X] Completada" : "[ ] Pendiente";
                            Console.WriteLine($"{t.Id}. {t.Descripcion} - {estado}");
                        }
                    }
                    break;

                case "3":
                    Console.Write("\nIngresa el ID de la tarea a completar: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        try
                        {
                            _servicio.MarcarComoTerminada (id);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("¡Tarea completada correctamente!");
                        }
                        catch (KeyNotFoundException ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        finally { Console.ResetColor(); }
                    }
                    else
                    {
                        Console.WriteLine("Por favor, ingresa un número de ID válido.");
                    }
                    break;

                case "4":
                    Console.Write("\nIngresa el ID de la tarea a eliminar: ");
                    if (int.TryParse(Console.ReadLine(), out int idEliminar))
                    {
                        try
                        {
                            _servicio.EliminiarTarea (idEliminar);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("¡Tarea eliminada correctamente!");
                        }
                        catch (KeyNotFoundException ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        finally { Console.ResetColor(); }
                    }
                    else
                    {
                        Console.WriteLine("Por favor, ingresa un número de ID válido.");
                    }
                    break;
                case "5":
                    continuar = false;
                    Console.WriteLine("\n¡Gracias por usar la aplicación!");
                    break;

                default:
                    Console.WriteLine("\nOpción inválida. Intenta de nuevo.");
                    break;
            }

            if (continuar)
            {
                Console.WriteLine("\nPresiona cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
    }
}
