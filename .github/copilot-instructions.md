## graphify

For any question about this repo's architecture, structure, components, or how to add/modify/find
code, your first action should be `graphify query "<question>"` when `graphify-out/graph.json`
exists. Use `graphify path "<A>" "<B>"` for relationship questions and `graphify explain "<concept>"`
for focused-concept questions. These return a scoped subgraph, usually much smaller than the full
report or raw grep output.

Triggers: "how do I…", "where is…", "what does … do", "add/modify a <component>",
"explain the architecture", or anything that depends on how files or classes relate.

If `graphify-out/wiki/index.md` exists, use it for broad navigation. Read `graphify-out/GRAPH_REPORT.md`
only for broad architecture review or when query/path/explain do not surface enough context. Only read
source files when (a) modifying/debugging specific code, (b) the graph lacks the needed detail, or
(c) the graph is missing or stale.

Type `/graphify` in Copilot Chat to build or update the graph.

## Required Graphify workflow

Before answering a codebase question or changing source code:

1. Refresh the graph from the current working tree with `graphify update .`. Do not rely on an existing graph until this succeeds. On this Windows installation, if `graphify` is not yet on the current process PATH, invoke it with `& "$HOME\.local\bin\graphify.exe" update .`.
2. Query the refreshed graph first with `graphify query "<question>"`. Use `graphify path "<A>" "<B>"` for relationships or `graphify explain "<concept>"` for one concept. Then inspect source files as needed before proposing or making changes. Use the same full executable path as above if `graphify` is not on PATH.
3. If the refresh fails, say the graph could not be refreshed; do not present its results as current. Continue with direct source inspection only when needed to complete the task.

After changing source files, run `graphify update .` again so subsequent agent turns see the updated graph (using the full executable path above if needed). Git hooks are a supplement, not a replacement for this workflow: they refresh after commits and branch switches, and may run asynchronously. After pulls or merges, refresh explicitly before querying.
