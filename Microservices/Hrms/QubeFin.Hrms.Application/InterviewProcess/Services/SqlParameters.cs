using System.Data;
using Microsoft.Data.SqlClient;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>Stored-procedure parameters for the optional list filters - a missing value goes to the SP as NULL.</summary>
internal static class SqlParameters
{
    public static SqlParameter Value(string name, object? value) => new(name, value ?? DBNull.Value);

    public static SqlParameter Text(string name, string? value) =>
        new(name, SqlDbType.NVarChar, 100) { Value = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim() };

    public static SqlParameter Date(string name, DateOnly? value) =>
        new(name, SqlDbType.Date) { Value = value is { } date ? date.ToDateTime(TimeOnly.MinValue) : DBNull.Value };
}
