using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiAppTerminal1.Interfaces;
using MiAppTerminal1.Infrastructure;
using MiAppTerminal1.Services;
using MiAppTerminal1.Api;
using Scalar.AspNetCore; // Requisito de infraestructura visual para .NET 10

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// DETECCIÓN DEL ENTORNO DE COMPILACIÓN (OPENAPI)
// ==========================================
// Comprobamos si el ensamblado ejecutor es el generador de archivos de OpenAPI de Microsoft
bool esGeneracionDeOpenApi = Environment.GetCommandLineArgs().Any(arg => arg.Contains("GetDocument.OpenApi"));

// ==========================================
// 1. REGISTRO DE DEPENDENCIAS (IoC / SOLID - D)
// ==========================================
builder.Services.AddScoped<ITareaRepository, JsonTareaRepository>();
builder.Services.AddScoped<TareaService>();

// CLEAN CODE: Registramos el motor nativo de OpenAPI de Microsoft (.NET 10)
builder.Services.AddOpenApi(); 

// PROTECCIÓN SENIOR: Aquí puedes registrar servicios pesados o externos (Bases de datos, APIs de terceros)
// para evitar que se ejecuten o pinchen durante el 'dotnet build'.
if (!esGeneracionDeOpenApi)
{
    // Ejemplo futuro: builder.Services.AddDbContext<MiContexto>();
}

var app = builder.Build();

// ==========================================
// 2. CONFIGURACIÓN DEL PIPELINE DE RED (MIDDLEWARES)
// ==========================================
if (app.Environment.IsDevelopment())
{
    // Herramientas de desarrollo aisladas por entorno
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();             // Genera el esquema JSON en /openapi/v1.json
    app.MapScalarApiReference();   // Renderiza el panel interactivo en /scalar/v1
}

// ==========================================
// 3. ORQUESTACIÓN MODULAR (SOLID - S / ARQUITECTURA LIMPIA)
// ==========================================
// El Program NO sabe qué rutas existen; delega la responsabilidad a la capa API
app.MapTareaEndpoints(); 

// 4. ARRANQUE DEL SERVIDOR
await app.RunAsync(); // Ejecución asíncronica nativa de extremo a extremo
