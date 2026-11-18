using System.Text.Json.Serialization;

namespace QubeFin.ImportEngine.Application.Imports.Models;

public enum ImportColumnType
{
    String,
    Number,
    Date
}

public class ImportColumnDefinition
{
    [JsonPropertyName("Header")]
    public string Header { get; set; } = null!;

    [JsonPropertyName("Param")]
    public string Param { get; set; } = null!;

    [JsonPropertyName("Type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ImportColumnType Type { get; set; }

    [JsonPropertyName("IsMandatory")]
    public bool IsMandatory { get; set; }
}
