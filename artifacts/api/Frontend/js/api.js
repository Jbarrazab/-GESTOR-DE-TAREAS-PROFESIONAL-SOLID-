const CONFIG = Object.freeze({
  baseUrl: globalThis.location?.origin ?? "http://localhost:5000",
  headers: Object.freeze({
    accept: "application/json",
    contentType: "application/json",
  }),
  endpoints: Object.freeze({
    tareas: "/api/tareas",
    tareaPorId: (id) => `/api/tareas/${encodeURIComponent(id)}`,
    completarTarea: (id) => `/api/tareas/completar/${encodeURIComponent(id)}`,
  }),
});

export const HTTP_STATUS = Object.freeze({
  badRequest: 400,
  unauthorized: 401,
  notFound: 404,
  internalServerError: 500,
});

export class ApiError extends Error {
  constructor(statusCode, traceId = null) {
    super(`La API respondió con el estado HTTP ${statusCode}.`);
    this.name = "ApiError";
    this.statusCode = statusCode;
    this.traceId = traceId;
  }
}

export class ApiNetworkError extends Error {
  constructor(cause) {
    super("No se pudo conectar con la API.", { cause });
    this.name = "ApiNetworkError";
  }
}

const buildUrl = (path) => `${CONFIG.baseUrl}${path}`;

const buildHeaders = (includesBody) => {
  const headers = new Headers({ Accept: CONFIG.headers.accept });

  if (includesBody) {
    headers.set("Content-Type", CONFIG.headers.contentType);
  }

  return headers;
};

const readTraceId = async (response) => {
  const responseText = await response.text();

  if (responseText.length === 0) {
    return null;
  }

  try {
    const errorResponse = JSON.parse(responseText);
    return typeof errorResponse.traceId === "string" ? errorResponse.traceId : null;
  } catch (error) {
    if (error instanceof SyntaxError) {
      return null;
    }

    throw error;
  }
};

const sendRequest = async (path, options = {}) => {
  let response;

  try {
    response = await fetch(buildUrl(path), {
      ...options,
      headers: buildHeaders(options.body !== undefined),
    });
  } catch (error) {
    if (error instanceof TypeError) {
      throw new ApiNetworkError(error);
    }

    throw error;
  }

  if (!response.ok) {
    throw new ApiError(response.status, await readTraceId(response));
  }

  if (response.status === 204) {
    return null;
  }

  try {
    return await response.json();
  } catch (error) {
    if (error instanceof SyntaxError) {
      throw new Error("La API devolvió una respuesta JSON no válida.", { cause: error });
    }

    throw error;
  }
};

const assertValidId = (id) => {
  const numericId = Number(id);

  if (!Number.isInteger(numericId) || numericId < 1) {
    throw new RangeError("El ID de la tarea debe ser un entero positivo.");
  }

  return numericId;
};

export const listarTareas = async () =>
  sendRequest(CONFIG.endpoints.tareas, { method: "GET" });

export const obtenerTareaPorId = async (id) =>
  sendRequest(CONFIG.endpoints.tareaPorId(assertValidId(id)), { method: "GET" });

export const crearTarea = async (request) =>
  sendRequest(CONFIG.endpoints.tareas, {
    method: "POST",
    body: JSON.stringify(request),
  });

export const completarTarea = async (id) =>
  sendRequest(CONFIG.endpoints.completarTarea(assertValidId(id)), { method: "PUT" });

export const actualizarTarea = async (id, request) =>
  sendRequest(CONFIG.endpoints.tareaPorId(assertValidId(id)), {
    method: "PUT",
    body: JSON.stringify(request),
  });

export const eliminarTarea = async (id) =>
  sendRequest(CONFIG.endpoints.tareaPorId(assertValidId(id)), { method: "DELETE" });
