using MiAppTerminal1.Infrastructure;
using MiAppTerminal1.Services;
using MiAppTerminal1.Ui;

// 1. Inicialización e Inyección de Dependencias
var repositorio = new MemoriaTareaRepository();
var servicio = new TareaService(repositorio);

// 2. Inicialización de la Capa de Presentación
var ui = new ConsoleUi(servicio);

// 3. Ejecución del programa
ui.EjecutarMenu();