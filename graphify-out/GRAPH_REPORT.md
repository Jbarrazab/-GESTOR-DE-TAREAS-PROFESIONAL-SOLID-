# Graph Report - MiAppTerminal1  (2026-10-14)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 131 nodes · 291 edges · 12 communities (8 shown, 4 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 30 edges (avg confidence: 0.82)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `01e8ba0f`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Task
- TareaService
- JsonTareaRepository
- Persona
- MiAppTerminal1
- MemoriaTareaRepository.cs
- TareaEndpoindts.cs
- MiAppTerminal1.Domain
- MiAppTerminal1.csproj
- ActualizarTareaRequest.cs
- JsonTareaRepository.cs

## God Nodes (most connected - your core abstractions)
1. `Tarea` - 28 edges
2. `TareaService` - 14 edges
3. `JsonTareaRepository` - 11 edges
4. `MemoriaTareaRepository` - 9 edges
5. `ITareaRepository` - 9 edges
6. `TareaEndpoints` - 9 edges
7. `MiAppTerminal1.Domain` - 9 edges
8. `Persona` - 8 edges
9. `IAsignable` - 7 edges
10. `TituloTarea` - 6 edges

## Surprising Connections (you probably didn't know these)
- `JsonTareaRepository` --implements--> `ITareaRepository`  [EXTRACTED]
  Infrastructure/JsonTareaRepository.cs → Interface/ITareaRepository.cs
- `TareaService` --references--> `ITareaRepository`  [EXTRACTED]
  Services/TareaService.cs → Interface/ITareaRepository.cs
- `MemoriaTareaRepository` --references--> `Tarea`  [EXTRACTED]
  Infrastructure/MemoriaTareaRepository.cs → Domain/Tarea.cs
- `MemoriaTareaRepository` --implements--> `ITareaRepository`  [EXTRACTED]
  Infrastructure/MemoriaTareaRepository.cs → Interface/ITareaRepository.cs
- `Tarea` --references--> `IAsignable`  [EXTRACTED]
  Domain/Tarea.cs → Domain/IAsignable.cs

## Import Cycles
- None detected.

## Communities (12 total, 4 thin omitted)

### Community 0 - "Task"
Cohesion: 0.18
Nodes (10): Tarea, Descripcion, ECompletado, Id, Responsable, Titulo, TituloTarea, Valor (+2 more)

### Community 1 - "TareaService"
Cohesion: 0.26
Nodes (4): MensajeResponse, TareaResponse, TareaEndpoints, TareaService

### Community 2 - "JsonTareaRepository"
Cohesion: 0.38
Nodes (3): TareaDto, ECompletadoLegacy, JsonTareaRepository

### Community 3 - "Persona"
Cohesion: 0.20
Nodes (10): IAsignable, Apellido, Identificador, Nombre, NombreVisual, Persona, Apellido, Identificador (+2 more)

### Community 4 - "MiAppTerminal1"
Cohesion: 0.20
Nodes (9): DOTNET_ENVIRONMENT, applicationUrl, commandName, environmentVariables, launchBrowser, launchUrl, profiles, MiAppTerminal1 (+1 more)

### Community 5 - "MemoriaTareaRepository.cs"
Cohesion: 0.33
Nodes (3): MiAppTerminal1.Infrastructure, MiAppTerminal1.Interfaces, MiAppTerminal1.Services

### Community 8 - "MiAppTerminal1.csproj"
Cohesion: 0.29
Nodes (6): net10.0, Microsoft.AspNetCore.OpenApi (10.0.0-rc.1.25451.107), Microsoft.Extensions.ApiDescription.Server (10.0.12), Microsoft.OpenApi (2.12.0), Scalar.AspNetCore (2.0.18), Microsoft.NET.Sdk.Web

### Community 10 - "ActualizarTareaRequest.cs"
Cohesion: 0.40
Nodes (3): ActualizarTareaRequest, CrearTareaRequest, MiAppTerminal1.Api.Requests

## Knowledge Gaps
- **21 isolated node(s):** `Descripcion`, `ECompletado`, `Id`, `Responsable`, `Titulo` (+16 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 42 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **4 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Tarea` connect `Task` to `TareaService`, `JsonTareaRepository`, `Persona`, `MiAppTerminal1.Domain`?**
  _High betweenness centrality (0.167) - this node is a cross-community bridge._
- **What connects `Descripcion`, `ECompletado`, `Id` to the rest of the system?**
  _21 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Why does `TareaEndpoints` connect `TareaService` to `TareaEndpoindts.cs`?**
  _High betweenness centrality (0.086) - this node is a cross-community bridge._
- **Why does `Persona` connect `Persona` to `Task`, `JsonTareaRepository`, `MiAppTerminal1.Domain`?**
  _High betweenness centrality (0.068) - this node is a cross-community bridge._