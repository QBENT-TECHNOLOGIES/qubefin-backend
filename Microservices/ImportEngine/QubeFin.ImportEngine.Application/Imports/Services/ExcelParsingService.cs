using ExcelDataReader;
using QubeFin.ImportEngine.Application.Imports.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace QubeFin.ImportEngine.Application.Imports.Services;

public class ExcelParsingService : IExcelParsingService
{
    private static readonly string[] AcceptedDateFormats = ["dd-MM-yyyy", "dd/MM/yyyy"];

    public ImportParseResult ParseWorkbook(Stream fileStream, List<ImportColumnDefinition> columns, int maxRows)
    {
        using var reader = ExcelReaderFactory.CreateReader(fileStream);

        if (!reader.Read())
        {
            return new ImportParseResult { MissingHeaders = { "File has no rows" } };
        }

        var rawHeaders = new string[reader.FieldCount];
        for (var i = 0; i < reader.FieldCount; i++)
        {
            rawHeaders[i] = reader.GetValue(i)?.ToString() ?? string.Empty;
        }

        var rawRows = new List<object?[]>();
        while (reader.Read())
        {
            var row = new object?[reader.FieldCount];
            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[i] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            rawRows.Add(row);
        }

        return ValidateAndMapRows(rawHeaders, rawRows, columns, maxRows);
    }

    public ImportParseResult ValidateAndMapRows(string[] rawHeaders, IEnumerable<object?[]> rawRows, List<ImportColumnDefinition> columns, int maxRows)
    {
        var result = new ImportParseResult();

        var normalizedHeaders = rawHeaders.Select(NormalizeHeader).ToArray();
        var headerIndexByColumn = new Dictionary<ImportColumnDefinition, int>();

        foreach (var column in columns)
        {
            var index = Array.IndexOf(normalizedHeaders, NormalizeHeader(column.Header));
            if (index < 0)
            {
                result.MissingHeaders.Add(column.Header);
            }
            else
            {
                headerIndexByColumn[column] = index;
            }
        }

        if (result.MissingHeaders.Count > 0)
        {
            return result;
        }

        var excelRowNumber = 1;
        foreach (var rawRow in rawRows)
        {
            excelRowNumber++;

            if (result.TotalRows >= maxRows)
            {
                result.Errors.Add(new ImportRowError { Row = excelRowNumber, Column = "", Message = $"File exceeds the maximum allowed rows ({maxRows})." });
                break;
            }

            if (IsRowEmpty(rawRow))
            {
                continue;
            }

            result.TotalRows++;

            var mappedRow = new Dictionary<string, string?>();
            foreach (var column in columns)
            {
                var index = headerIndexByColumn[column];
                var rawValue = index < rawRow.Length ? rawRow[index] : null;

                var (value, error) = ConvertCell(rawValue, column);
                if (error is not null)
                {
                    result.Errors.Add(new ImportRowError
                    {
                        Row = excelRowNumber,
                        Column = column.Header,
                        Value = rawValue?.ToString(),
                        Message = error
                    });
                    continue;
                }

                mappedRow[column.Param] = value;
            }

            result.Rows.Add(mappedRow);
        }

        return result;
    }

    private static bool IsRowEmpty(object?[] row)
    {
        return row.All(cell => cell is null || string.IsNullOrWhiteSpace(cell.ToString()));
    }

    private static (string? Value, string? Error) ConvertCell(object? rawValue, ImportColumnDefinition column)
    {
        var text = rawValue?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return (null, null);
        }

        return column.Type switch
        {
            ImportColumnType.String => (text.Trim(), null),
            ImportColumnType.Number => ParseNumber(text),
            ImportColumnType.Date => ParseDate(rawValue, text),
            _ => (null, $"Unsupported column type for '{column.Header}'.")
        };
    }

    private static (string? Value, string? Error) ParseNumber(string text)
    {
        if (!double.TryParse(text.Trim(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var number))
        {
            return (null, $"'{text}' is not a valid number.");
        }

        return (number.ToString(CultureInfo.InvariantCulture), null);
    }

    private static (string? Value, string? Error) ParseDate(object? rawValue, string text)
    {
        if (rawValue is DateTime dateTime)
        {
            return (dateTime.ToString("yyyy-MM-dd"), null);
        }

        if (double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var serial) && serial > 59)
        {
            try
            {
                return (DateTime.FromOADate(serial).ToString("yyyy-MM-dd"), null);
            }
            catch (ArgumentException)
            {
                // Not a valid OLE Automation date serial, fall through to exact-format parsing.
            }
        }

        if (DateTime.TryParseExact(text.Trim(), AcceptedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return (parsed.ToString("yyyy-MM-dd"), null);
        }

        return (null, $"'{text}' is not a valid date. Expected dd-MM-yyyy or dd/MM/yyyy.");
    }

    private static string NormalizeHeader(string header)
    {
        return Regex.Replace(header.Trim(), @"\s+", " ").ToLowerInvariant();
    }
}
