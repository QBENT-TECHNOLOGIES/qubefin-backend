using QubeFin.ImportEngine.Application.Imports.Models;

namespace QubeFin.ImportEngine.Application.Imports.Services;

public interface IExcelParsingService
{
    ImportParseResult ParseWorkbook(Stream fileStream, List<ImportColumnDefinition> columns, int maxRows);

    ImportParseResult ValidateAndMapRows(string[] rawHeaders, IEnumerable<object?[]> rawRows, List<ImportColumnDefinition> columns, int maxRows);
}
