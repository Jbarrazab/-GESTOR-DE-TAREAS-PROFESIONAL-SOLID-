
$env:DOTNET_ENVIRONMENT="Production/Development"
dotnet run

============================================================================
Cambia a Production de forma nativa: Al no leer el JSON de desarrollo, .NET no encuentra la variable Development y arranca por defecto en el entorno seguro de Production.

dotnet run --launch-profile ""
============================================================================dotnet run --no-launch-profile -e DOTNET_ENVIRONMENT=Production
