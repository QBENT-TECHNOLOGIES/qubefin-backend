using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace QubeFin.Payroll.Persistence.Repositories.ExcelHelpers
{
    public static class ReportDataHelper
    {
        public static async Task<DataTable> ExecuteStoredProcedureAsync(string connectionString, string storedProcedure, Dictionary<string, object?> parameters, CancellationToken cancellationToken)
        {
            var dataTable = new DataTable();

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            ReportParameterHelper.AddParameters(command, parameters);

            await connection.OpenAsync(cancellationToken);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            dataTable.Load(reader);

            return dataTable;
        }
    }
}
