using Microsoft.Data.SqlClient;
using System.Data;

namespace QubeFin.Payroll.Persistence.Repositories.ExcelHelpers
{
    public static class ReportParameterHelper
    {
        public static void AddParameters(SqlCommand command, Dictionary<string, object?> parameters)
        {
            foreach (var parameter in parameters)
            {
                var value = parameter.Value ?? DBNull.Value;

                command.Parameters.Add(CreateParameter(command, parameter.Key, value));
            }
        }

        private static SqlParameter CreateParameter(SqlCommand command, string name, object value)
        {
            var parameter = new SqlParameter
            {
                ParameterName = name,
                Value = value
            };

            switch (value)
            {
                case Guid:
                    parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                    break;

                case int:
                    parameter.SqlDbType = SqlDbType.Int;
                    break;

                case long:
                    parameter.SqlDbType = SqlDbType.BigInt;
                    break;

                case short:
                    parameter.SqlDbType = SqlDbType.SmallInt;
                    break;

                case bool:
                    parameter.SqlDbType = SqlDbType.Bit;
                    break;

                case DateTime:
                    parameter.SqlDbType = SqlDbType.DateTime2;
                    break;

                case DateOnly dateOnly:
                    parameter.SqlDbType = SqlDbType.Date;
                    parameter.Value = dateOnly.ToDateTime(TimeOnly.MinValue);
                    break;

                case decimal:
                    parameter.SqlDbType = SqlDbType.Decimal;
                    break;

                case double:
                    parameter.SqlDbType = SqlDbType.Float;
                    break;

                case string:
                    parameter.SqlDbType = SqlDbType.NVarChar;
                    parameter.Size = -1;
                    break;

                case DBNull:
                    // When the value is null, SQL Server may infer the
                    // parameter type from the stored procedure.
                    break;
            }

            return parameter;
        }
    }
}
