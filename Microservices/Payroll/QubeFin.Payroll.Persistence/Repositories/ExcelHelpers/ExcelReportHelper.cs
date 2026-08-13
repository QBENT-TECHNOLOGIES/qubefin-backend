using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Text.RegularExpressions;

namespace QubeFin.Payroll.Persistence.Repositories.ExcelHelpers
{
    public static class ExcelReportHelper
    {
        public record ExcelReportOptions(bool ShowCompanyHeader = false, string? ReportTitle = null, string? SubHeader = null);
        public static MemoryStream CreateExcel(DataTable dataTable, ExcelReportOptions options, byte[]? logoBytes)
        {
            var workbook = new XSSFWorkbook();

            try
            {
                var sheet = workbook.CreateSheet("Report");

                var currentRow = 0;

                // Company Header
                if (options.ShowCompanyHeader)
                {
                    currentRow = ExcelCompanyHeaderHelper.AddCompanyHeader(workbook, sheet, currentRow, dataTable.Columns.Count, logoBytes);
                }

                // Report Title
                if (!string.IsNullOrWhiteSpace(options.ReportTitle))
                {
                    AddHeader(workbook, sheet, ref currentRow, options.ReportTitle, dataTable.Columns.Count);
                }

                // Report Sub Header
                if (!string.IsNullOrWhiteSpace(options.SubHeader))
                {
                    AddSubHeader(workbook, sheet, ref currentRow, options.SubHeader, dataTable.Columns.Count);
                }

                // Column Headers
                AddColumnHeaders(workbook, sheet, ref currentRow, dataTable);

                // Data
                if (dataTable.Rows.Count == 0)
                {
                    AddNoDataRow(workbook, sheet, ref currentRow, dataTable.Columns.Count);
                }
                else
                {
                    AddData(workbook, sheet, ref currentRow, dataTable);
                }

                // Auto Size
                AutoSizeColumns(sheet, dataTable.Columns.Count);

                var stream = new MemoryStream();

                workbook.Write(stream, leaveOpen: true);

                stream.Position = 0;

                return stream;
            }
            finally
            {
                workbook.Close();
            }
        }
        private static void AddHeader(IWorkbook workbook, ISheet sheet, ref int currentRow, string header, int columnCount)
        {
            var row = sheet.CreateRow(currentRow++);
            row.HeightInPoints = 25;

            var cell = row.CreateCell(0);
            cell.SetCellValue(header);
            cell.CellStyle = CreateHeaderStyle(workbook);

            MergeCells(sheet, row.RowNum, columnCount);
        }

        private static void AddSubHeader(IWorkbook workbook, ISheet sheet, ref int currentRow, string subHeader, int columnCount)
        {
            var row = sheet.CreateRow(currentRow++);
            row.HeightInPoints = 20;

            var cell = row.CreateCell(0);
            cell.SetCellValue(subHeader);
            cell.CellStyle = CreateSubHeaderStyle(workbook);

            MergeCells(sheet, row.RowNum, columnCount);
        }

        private static void AddColumnHeaders(IWorkbook workbook, ISheet sheet, ref int currentRow, DataTable dataTable)
        {
            var row = sheet.CreateRow(currentRow++);

            var style = CreateColumnHeaderStyle(workbook);

            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(ToDisplayName(dataTable.Columns[i].ColumnName));
                cell.CellStyle = style;
            }
        }

        private static void AddData(IWorkbook workbook, ISheet sheet, ref int currentRow, DataTable dataTable)
        {
            var dateStyle = workbook.CreateCellStyle();

            dateStyle.DataFormat = workbook
                .CreateDataFormat()
                .GetFormat("dd/MM/yyyy");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var row = sheet.CreateRow(currentRow++);

                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    var cell = row.CreateCell(i);

                    ExcelCellHelper.SetValue(
                        cell,
                        dataRow[i],
                        dateStyle);
                }
            }
        }

        private static void AddNoDataRow(IWorkbook workbook, ISheet sheet, ref int currentRow, int columnCount)
        {
            var row = sheet.CreateRow(currentRow++);

            var cell = row.CreateCell(0);

            cell.SetCellValue("No data available");

            var style = workbook.CreateCellStyle();

            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            var font = workbook.CreateFont();
            font.IsItalic = true;

            style.SetFont(font);

            cell.CellStyle = style;

            if (columnCount > 1)
            {
                sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, 0, columnCount - 1));
            }
        }

        private static void AutoSizeColumns(ISheet sheet, int columnCount)
        {
            for (var i = 0; i < columnCount; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        private static void MergeCells(ISheet sheet, int row, int columnCount)
        {
            if (columnCount > 1)
            {
                sheet.AddMergedRegion(new CellRangeAddress(row, row, 0, columnCount - 1));
            }
        }

        private static ICellStyle CreateHeaderStyle(IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            var font = workbook.CreateFont();
            font.IsBold = true;
            font.FontHeightInPoints = 16;

            style.SetFont(font);

            return style;
        }

        private static ICellStyle CreateSubHeaderStyle(IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            var font = workbook.CreateFont();
            font.IsBold = true;
            font.FontHeightInPoints = 11;

            style.SetFont(font);

            return style;
        }

        private static ICellStyle CreateColumnHeaderStyle(IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            var font = workbook.CreateFont();
            font.IsBold = true;

            style.SetFont(font);

            return style;
        }
        public static string ToDisplayName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            value = value.Trim().Replace("_", " ").Replace("-", " ");
            value = Regex.Replace(value, @"(?<=[a-z])(?=[A-Z])", " ");
            value = Regex.Replace(value, @"(?<=[A-Z])(?=[A-Z][a-z])", " ");
            value = Regex.Replace(value, @"\s+", " ");

            return value.Trim();
        }
    }
}
