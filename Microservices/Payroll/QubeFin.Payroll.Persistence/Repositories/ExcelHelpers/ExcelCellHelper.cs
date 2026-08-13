using NPOI.SS.UserModel;

namespace QubeFin.Payroll.Persistence.Repositories.ExcelHelpers
{
    public static class ExcelCellHelper
    {
        public static void SetValue(
    ICell cell,
    object? value,
    ICellStyle? dateStyle = null)
        {
            if (value == null || value == DBNull.Value)
            {
                cell.SetCellValue(string.Empty);
                return;
            }

            switch (value)
            {
                case DateTime dateTime:
                    cell.SetCellValue(dateTime);

                    if (dateStyle != null)
                        cell.CellStyle = dateStyle;

                    break;

                case DateOnly dateOnly:
                    cell.SetCellValue(
                        dateOnly.ToDateTime(TimeOnly.MinValue));

                    if (dateStyle != null)
                        cell.CellStyle = dateStyle;

                    break;

                case int intValue:
                    cell.SetCellValue(intValue);
                    break;

                case long longValue:
                    cell.SetCellValue(longValue);
                    break;

                case decimal decimalValue:
                    cell.SetCellValue((double)decimalValue);
                    break;

                case double doubleValue:
                    cell.SetCellValue(doubleValue);
                    break;

                case bool boolValue:
                    cell.SetCellValue(boolValue);
                    break;

                default:
                    cell.SetCellValue(value.ToString());
                    break;
            }
        }
    }
}
