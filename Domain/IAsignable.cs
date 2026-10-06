using System.Text.Json.Serialization;
namespace MiAppTerminal1.Domain;

// 1. Le indicamos al serializador que use un discriminador de tipo $type en el JSON
[JsonDerivedType(typeof(Persona), typeDiscriminator: "persona")]
// [JsonDerivedType(typeof(Equipo), typeDiscriminator: "equipo")] <-- Así de fácil se escalaría en el futuro



public interface IAsignable
{
    string Identificador {get; }
    string NombreVisual {get; }

}
