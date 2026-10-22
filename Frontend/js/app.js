import {
  ApiError,
  ApiNetworkError,
  HTTP_STATUS,
  actualizarTarea,
  completarTarea,
  crearTarea,
  eliminarTarea,
  listarTareas,
  obtenerTareaPorId,
} from "./api.js";

const ELEMENT_IDS = Object.freeze({
  createForm: "create-task-form",
  findForm: "find-task-form",
  refreshButton: "refresh-tasks-button",
  tableBody: "tasks-table-body",
  emptyState: "empty-state",
  statusMessage: "status-message",
  editDialog: "edit-task-dialog",
  editForm: "edit-task-form",
  cancelEditButton: "cancel-edit-button",
  deleteDialog: "delete-task-dialog",
  deletePrompt: "delete-task-prompt",
  cancelDeleteButton: "cancel-delete-button",
  confirmDeleteButton: "confirm-delete-button",
});

const ERROR_MESSAGES = Object.freeze({
  [HTTP_STATUS.badRequest]: "Los datos enviados no son válidos. Revisa los campos e inténtalo de nuevo.",
  [HTTP_STATUS.unauthorized]: "La API requiere autenticación para realizar esta operación.",
  [HTTP_STATUS.notFound]: "No se encontró la tarea solicitada.",
  [HTTP_STATUS.internalServerError]: "La API encontró un error interno. Inténtalo más tarde.",
});

const elements = Object.fromEntries(
  Object.entries(ELEMENT_IDS).map(([key, id]) => [key, document.getElementById(id)]),
);

for (const [key, element] of Object.entries(elements)) {
  if (element === null) {
    throw new Error(`No se encontró en el documento el elemento requerido: ${ELEMENT_IDS[key]}.`);
  }
}

let pendingDeleteId = null;

const setStatusMessage = (message, isError = false) => {
  const statusMessage = elements.statusMessage;
  statusMessage.textContent = message;
  statusMessage.className = isError
    ? "rounded-lg border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-800"
    : "rounded-lg border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-800";
};

const clearStatusMessage = () => {
  elements.statusMessage.textContent = "";
  elements.statusMessage.className = "hidden rounded-lg border px-4 py-3 text-sm";
};

const getErrorMessage = (error) => {
  if (error instanceof ApiNetworkError) {
    return "No se pudo conectar con la API. Comprueba que el backend esté ejecutándose.";
  }

  if (error instanceof ApiError) {
    const knownMessage = ERROR_MESSAGES[error.statusCode];
    const message = knownMessage ?? `La API devolvió el estado HTTP ${error.statusCode}.`;
    return error.traceId ? `${message} Referencia: ${error.traceId}` : message;
  }

  if (error instanceof RangeError) {
    return error.message;
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "Ocurrió un error inesperado al comunicarse con la API.";
};

const runSafely = async (operation) => {
  clearStatusMessage();

  try {
    await operation();
  } catch (error) {
    setStatusMessage(getErrorMessage(error), true);
  }
};

const createCell = (text, className = "px-4 py-3 align-top") => {
  const cell = document.createElement("td");
  cell.className = className;
  cell.textContent = text;
  return cell;
};

const createActionButton = (label, action, taskId, isDestructive = false) => {
  const button = document.createElement("button");
  button.type = "button";
  button.dataset.action = action;
  button.dataset.taskId = String(taskId);
  button.textContent = label;
  button.className = isDestructive
    ? "font-medium text-rose-700 underline-offset-2 hover:underline focus:outline-none focus:ring-2 focus:ring-rose-500"
    : "font-medium text-indigo-700 underline-offset-2 hover:underline focus:outline-none focus:ring-2 focus:ring-indigo-500";
  return button;
};

const createTaskRow = (task) => {
  const row = document.createElement("tr");
  row.append(
    createCell(String(task.id), "whitespace-nowrap px-4 py-3 align-top font-mono text-xs text-slate-500"),
  );

  const taskCell = document.createElement("td");
  taskCell.className = "max-w-md px-4 py-3 align-top";
  const title = document.createElement("p");
  title.className = "font-semibold text-slate-900";
  title.textContent = task.titulo;
  const description = document.createElement("p");
  description.className = "mt-1 whitespace-pre-wrap text-slate-600";
  description.textContent = task.descripcion;
  taskCell.append(title, description);
  row.append(taskCell);

  const assignee = [task.nombreResponsable, task.apellidoResponsable].filter(Boolean).join(" ");
  row.append(createCell(assignee));

  const statusCell = document.createElement("td");
  statusCell.className = "whitespace-nowrap px-4 py-3 align-top";
  const statusBadge = document.createElement("span");
  statusBadge.className = task.estaCompletado
    ? "inline-flex rounded-full bg-emerald-100 px-2.5 py-1 text-xs font-semibold text-emerald-800"
    : "inline-flex rounded-full bg-amber-100 px-2.5 py-1 text-xs font-semibold text-amber-800";
  statusBadge.textContent = task.estaCompletado ? "Completada" : "Pendiente";
  statusCell.append(statusBadge);
  row.append(statusCell);

  const actionsCell = document.createElement("td");
  actionsCell.className = "whitespace-nowrap px-4 py-3 align-top";
  const actions = document.createElement("div");
  actions.className = "flex flex-wrap gap-x-3 gap-y-2";
  if (!task.estaCompletado) {
    actions.append(createActionButton("Completar", "complete", task.id));
  }
  actions.append(
    createActionButton("Editar", "edit", task.id),
    createActionButton("Eliminar", "delete", task.id, true),
  );
  actionsCell.append(actions);
  row.append(actionsCell);

  return row;
};

const renderTasks = (tasks) => {
  const tableBody = elements.tableBody;
  tableBody.replaceChildren();

  if (!Array.isArray(tasks)) {
    throw new TypeError("La API no devolvió una lista de tareas.");
  }

  const rows = tasks.map(createTaskRow);
  tableBody.append(...rows);
  elements.emptyState.classList.toggle("hidden", tasks.length > 0);
};

const loadTasks = async () => {
  const tasks = await listarTareas();
  renderTasks(tasks);
  const taskLabel = tasks.length === 1 ? "tarea" : "tareas";
  setStatusMessage(`Se cargaron ${tasks.length} ${taskLabel}.`);
};

const getFormValue = (formData, fieldName) => {
  const value = formData.get(fieldName);
  return typeof value === "string" ? value.trim() : "";
};

const handleCreateTask = async (event) => {
  event.preventDefault();
  const form = event.currentTarget;
  const formData = new FormData(form);
  const request = {
    titulo: getFormValue(formData, "titulo"),
    descripcion: getFormValue(formData, "descripcion"),
    nombreResponsable: getFormValue(formData, "nombreResponsable"),
    apellidoResponsable: getFormValue(formData, "apellidoResponsable"),
  };

  await runSafely(async () => {
    const task = await crearTarea(request);
    form.reset();
    await loadTasks();
    setStatusMessage(`Se creó la tarea «${task.titulo}» (ID ${task.id}).`);
  });
};

const handleFindTask = async (event) => {
  event.preventDefault();
  const formData = new FormData(event.currentTarget);
  const taskId = getFormValue(formData, "id");

  await runSafely(async () => {
    const task = await obtenerTareaPorId(taskId);
    renderTasks([task]);
    setStatusMessage(`Se encontró la tarea con ID ${task.id}.`);
  });
};

const findTaskInTable = (taskId) => {
  const rows = [...elements.tableBody.querySelectorAll("tr")];
  const taskRow = rows.find((row) => row.firstElementChild?.textContent === String(taskId));
  return taskRow;
};

const openEditDialog = (taskId) => {
  const taskRow = findTaskInTable(taskId);
  if (!taskRow) {
    setStatusMessage("Actualiza la lista antes de editar esta tarea.", true);
    return;
  }

  const editForm = elements.editForm;
  const formElements = editForm.elements;
  formElements.namedItem("id").value = taskId;
  formElements.namedItem("nuevoTitulo").value = taskRow.children[1].querySelector("p").textContent;
  formElements.namedItem("nuevaDescripcion").value =
    taskRow.children[1].querySelectorAll("p")[1].textContent;
  elements.editDialog.showModal();
};

const handleEditTask = async (event) => {
  event.preventDefault();
  const form = event.currentTarget;
  const formData = new FormData(form);
  const taskId = getFormValue(formData, "id");
  const request = {
    nuevoTitulo: getFormValue(formData, "nuevoTitulo"),
    nuevaDescripcion: getFormValue(formData, "nuevaDescripcion"),
  };

  await runSafely(async () => {
    await actualizarTarea(taskId, request);
    elements.editDialog.close();
    await loadTasks();
    setStatusMessage("La tarea se actualizó correctamente.");
  });
};

const openDeleteDialog = (taskId) => {
  pendingDeleteId = taskId;
  elements.deletePrompt.textContent = `¿Quieres eliminar la tarea con ID ${taskId}? Esta acción no se puede deshacer.`;
  elements.deleteDialog.showModal();
};

const handleDeleteTask = async () => {
  if (pendingDeleteId === null) {
    return;
  }

  const taskId = pendingDeleteId;
  await runSafely(async () => {
    await eliminarTarea(taskId);
    elements.deleteDialog.close();
    pendingDeleteId = null;
    await loadTasks();
    setStatusMessage(`Se eliminó la tarea con ID ${taskId}.`);
  });
};

const handleTaskAction = async (event) => {
  const button = event.target.closest("button[data-action]");
  if (!button) {
    return;
  }

  const { action, taskId } = button.dataset;
  if (!taskId) {
    setStatusMessage("No se pudo determinar el ID de la tarea.", true);
    return;
  }

  if (action === "edit") {
    openEditDialog(taskId);
    return;
  }

  if (action === "delete") {
    openDeleteDialog(taskId);
    return;
  }

  if (action === "complete") {
    await runSafely(async () => {
      await completarTarea(taskId);
      await loadTasks();
      setStatusMessage(`Se completó la tarea con ID ${taskId}.`);
    });
  }
};

elements.createForm.addEventListener("submit", handleCreateTask);
elements.findForm.addEventListener("submit", handleFindTask);
elements.refreshButton.addEventListener("click", () => runSafely(loadTasks));
elements.tableBody.addEventListener("click", (event) => runSafely(() => handleTaskAction(event)));
elements.editForm.addEventListener("submit", handleEditTask);
elements.cancelEditButton.addEventListener("click", () => elements.editDialog.close());
elements.cancelDeleteButton.addEventListener("click", () => {
  elements.deleteDialog.close();
  pendingDeleteId = null;
});
elements.confirmDeleteButton.addEventListener("click", handleDeleteTask);

await runSafely(loadTasks);
