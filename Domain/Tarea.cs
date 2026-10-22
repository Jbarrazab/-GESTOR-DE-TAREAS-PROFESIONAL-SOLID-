
namespace MiAppTerminal1.Domain;

public class Tarea
{
    public int Id { get; private set; }
    public TituloTarea Titulo { get; private set; }
    public string Descripcion { get; private set; }
    public IAsignable Responsable { get; private set; }
    public bool ECompletado { get; private set; }

    public Tarea(
        int id,
        TituloTarea titulo,
        string descripcion,
        IAsignable responsable,
        bool estaCompletada = false)
    {
        if (id < 0) throw new ArgumentOutOfRangeException(nameof(id), "El ID no puede ser negativo.");

        Id = id;
        Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
        Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
        Responsable = responsable ?? throw new ArgumentNullException(nameof(responsable));
        ECompletado = estaCompletada;
    }

    internal void AsignarId(int id)
    {
        if (Id != 0) throw new InvalidOperationException("El ID de la tarea ya fue asignado.");
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor a cero.");

        Id = id;
    }

    public void MarcarComoCompletada() => ECompletado = true;

    public void Actualizar(TituloTarea titulo, string descripcion)
    {
        Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
        Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
    }
}
