using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiAppTerminal1.Interfaces;
using MiAppTerminal1.Infrastructure;
using MiAppTerminal1.Services;
using MiAppTerminal1.Ui;

// 1. Inicializamos el Host de .NET moderno
var builder =  Host.CreateApplicationBuilder(args);

// 2. Registro Central de Dependencias (Contenedor de Inyección Automática)
// Configuramos la persistencia en JSON y la lógica de negocio usando el ciclo de vida Scoped
builder.Services.AddScoped<ITareaRepository, JsonTareaRepository>(); 
builder.Services.AddScoped<TareaService>();

// Registramos tu componente de interfaz de usuario de consola
builder.Services.AddTransient<ConsoleUi>();

// Construimos el Host con los servicios compilados en el contenedor
var app = builder.Build();

// 3. Punto de Entrada Limpio con Alcance (Scope) de Ejecución
// Creamos un Scope seguro para resolver e iniciar los componentes principales
using (var scope = app.Services.CreateScope())
{
    // El contenedor analiza ConsoleUi, detecta sus necesidades en cascada y resuelve el árbol de objetos
    var uiPrincipal = scope.ServiceProvider.GetRequiredService<ConsoleUi>();

    // Ejecutamos tu menú interactivo corregido
    uiPrincipal.EjecutarMenu();
}
