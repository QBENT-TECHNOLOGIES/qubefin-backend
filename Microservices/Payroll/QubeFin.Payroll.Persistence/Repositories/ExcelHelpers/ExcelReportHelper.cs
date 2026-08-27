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
                ApplyColumnWidths(sheet, dataTable);

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

        public static MemoryStream CreateBankSalaryDisbursementExcel(
            DataTable dataTable,
            byte[]? logoBytes,
            int month,
            int year,
            string name,
            string designation,
            string code)
        {
            var workbook = new XSSFWorkbook();

            try
            {
                var sheet = workbook.CreateSheet("Report");

                // Same existing column width logic
                ApplyColumnWidths(sheet, dataTable);

                var currentRow = 0;

                // =========================================================
                // COMPANY HEADER - SAME AS EXISTING
                // =========================================================

                currentRow = ExcelCompanyHeaderHelper.AddCompanyLogo(
                    workbook,
                    sheet,
                    currentRow,
                    dataTable.Columns.Count,
                    logoBytes);

                // =========================================================
                // REPORT TITLE - SAME AS EXISTING
                // =========================================================

                AddHeader(
                    workbook,
                    sheet,
                    ref currentRow,
                    "Bank Salary Disbursement Sheet",
                    dataTable.Columns.Count);

                // =========================================================
                // SUB HEADER - SAME AS EXISTING
                // =========================================================

                var monthName = GetPaymentMonth(month, year);

                AddSubHeader(
                    workbook,
                    sheet,
                    ref currentRow,
                    $"For the month of {monthName}",
                    dataTable.Columns.Count);

                // =========================================================
                // COLUMN HEADERS - SAME AS EXISTING
                // =========================================================

                AddColumnHeaders(
                    workbook,
                    sheet,
                    ref currentRow,
                    dataTable);

                // =========================================================
                // EXAMPLE ROW - bank salary sheet only
                // =========================================================

                //AddExampleRow(workbook, sheet, ref currentRow);

                // =========================================================
                // DATA - SAME AS EXISTING
                // =========================================================

                if (dataTable.Rows.Count == 0)
                {
                    AddNoDataRow(
                        workbook,
                        sheet,
                        ref currentRow,
                        dataTable.Columns.Count);
                }
                else
                {
                    AddData(
                        workbook,
                        sheet,
                        ref currentRow,
                        dataTable);

                    AddBankSalaryTotal(
                        workbook,
                        sheet,
                        ref currentRow,
                        dataTable);
                }

                // Footer should always be shown
                AddBankSalaryFooter(
                    workbook,
                    sheet,
                    ref currentRow,
                    dataTable.Columns.Count,
                    name,
                    designation,
                    code);

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

        private static string GetPaymentMonth(int month, int year)
        {
            if (month < 1 || month > 12)
                return string.Empty;

            if (year < 1)
                return string.Empty;

            return new DateTime(year, month, 1).ToString("MMMM yyyy");
        }

        // =============================================================
        // EXAMPLE ROW
        // =============================================================

        private static void AddExampleRow(IWorkbook workbook, ISheet sheet, ref int currentRow)
        {
            var row = sheet.CreateRow(currentRow++);

            var cell = row.CreateCell(0);
            cell.SetCellValue("Example");
            cell.CellStyle = CreateExampleRowStyle(workbook);
        }

        private static ICellStyle CreateExampleRowStyle(IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;

            var font = workbook.CreateFont();
            font.Color = IndexedColors.Blue.Index;

            style.SetFont(font);

            return style;
        }

        // =============================================================
        // TOTAL ROW
        // =============================================================

        private static void AddBankSalaryTotal(
            IWorkbook workbook,
            ISheet sheet,
            ref int currentRow,
            DataTable dataTable)
        {
            if (!dataTable.Columns.Contains("Amount"))
                return;

            var amountColumnIndex = dataTable.Columns.IndexOf("Amount");

            decimal totalAmount = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                if (row["Amount"] == DBNull.Value)
                    continue;

                if (decimal.TryParse(row["Amount"]?.ToString(), out var amount))
                {
                    totalAmount += amount;
                }
            }

            var totalRow = sheet.CreateRow(currentRow++);
            var labelStyle = CreateBankSalaryTotalStyle(workbook);

            // Total label
            var totalCell = totalRow.CreateCell(0);
            totalCell.SetCellValue("Total");
            totalCell.CellStyle = labelStyle;

            // Every cell in the merged range needs the border style applied
            // directly — a merge only carries the top-left cell's value,
            // it does not extend that cell's border to the rest of the range.
            for (var i = 1; i < amountColumnIndex; i++)
            {
                totalRow.CreateCell(i).CellStyle = labelStyle;
            }

            // Merge all columns before Amount
            if (amountColumnIndex > 0)
            {
                sheet.AddMergedRegion(
                    new CellRangeAddress(
                        totalRow.RowNum,
                        totalRow.RowNum,
                        0,
                        amountColumnIndex - 1));
            }

            // Amount
            var amountCell = totalRow.CreateCell(amountColumnIndex);
            amountCell.SetCellValue((double)totalAmount);
            amountCell.CellStyle = CreateBankSalaryTotalAmountStyle(workbook);

            // Remaining cells
            for (var i = amountColumnIndex + 1; i < dataTable.Columns.Count; i++)
            {
                totalRow.CreateCell(i).CellStyle = labelStyle;
            }
        }

        private static ICellStyle CreateBankSalaryTotalStyle(IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;

            var font = workbook.CreateFont();
            font.IsBold = true;

            style.SetFont(font);

            return style;
        }

        private static ICellStyle CreateBankSalaryTotalAmountStyle(IWorkbook workbook)
        {
            var style = CreateBankSalaryTotalStyle(workbook);

            style.Alignment = HorizontalAlignment.Right;

            style.DataFormat = workbook
                .CreateDataFormat()
                .GetFormat("0.00");

            return style;
        }

        // =============================================================
        // FOOTER (Prepared By / Checked By)
        // =============================================================

        private static void AddBankSalaryFooter(
            IWorkbook workbook,
            ISheet sheet,
            ref int currentRow,
            int columnCount,
            string preparedByName,
            string preparedByDesignation,
            string preparedByCode)
        {
            // Space between total/no-data row and the signature block
            currentRow += 2;

            var preparedStart = 1; // Date of Payment
            var preparedEnd = 2;   // Company Code    (2 cols wide)
            var checkedEnd = columnCount - 1;    // Mode of Payment
            var checkedStart = columnCount - 2;  // SBI/Non-SBI     (2 cols wide)

            var lineStyle = CreateFooterBorderStyle(workbook);

            var centerLabelStyle = CreateBankSalaryFooterStyle(workbook);
            centerLabelStyle.Alignment = HorizontalAlignment.Center;

            // =========================================================
            // Signature line — its own row, so it can never collide with
            // (and overwrite) the label text below it
            // =========================================================

            var lineRow = sheet.CreateRow(currentRow++);

            for (var i = preparedStart; i <= preparedEnd; i++)
                lineRow.CreateCell(i).CellStyle = lineStyle;

            for (var i = checkedStart; i <= checkedEnd; i++)
                lineRow.CreateCell(i).CellStyle = lineStyle;

            // =========================================================
            // "Prepared By" / "Checked By" — merged + centered across
            // the same two columns as the line above them
            // =========================================================

            var labelRow = sheet.CreateRow(currentRow++);

            var preparedCell = labelRow.CreateCell(preparedStart);
            preparedCell.SetCellValue("Prepared By");
            preparedCell.CellStyle = centerLabelStyle;
            for (var i = preparedStart + 1; i <= preparedEnd; i++)
                labelRow.CreateCell(i).CellStyle = centerLabelStyle;
            sheet.AddMergedRegion(new CellRangeAddress(labelRow.RowNum, labelRow.RowNum, preparedStart, preparedEnd));

            var checkedCell = labelRow.CreateCell(checkedStart);
            checkedCell.SetCellValue("Checked By");
            checkedCell.CellStyle = centerLabelStyle;
            for (var i = checkedStart + 1; i <= checkedEnd; i++)
                labelRow.CreateCell(i).CellStyle = centerLabelStyle;
            sheet.AddMergedRegion(new CellRangeAddress(labelRow.RowNum, labelRow.RowNum, checkedStart, checkedEnd));

            currentRow += 2;

            // =========================================================
            // Name / Designation / Employee Code
            // Prepared By side gets the actual values; Checked By side
            // gets labels only, to be filled in later by the checker.
            // =========================================================

            var fieldLabelStyle = CreateBankSalaryFooterStyle(workbook);
            var fieldValueStyle = CreateBankSalaryFooterValueStyle(workbook);

            AddBankSalaryFooterField(sheet, currentRow, preparedStart, "Name", preparedByName, fieldLabelStyle, fieldValueStyle);
            AddBankSalaryFooterField(sheet, currentRow + 1, preparedStart, "Designation", preparedByDesignation, fieldLabelStyle, fieldValueStyle);
            AddBankSalaryFooterField(sheet, currentRow + 2, preparedStart, "Employee Code", preparedByCode, fieldLabelStyle, fieldValueStyle);

            AddBankSalaryFooterField(sheet, currentRow, checkedStart, "Name", null, fieldLabelStyle, fieldValueStyle);
            AddBankSalaryFooterField(sheet, currentRow + 1, checkedStart, "Designation", null, fieldLabelStyle, fieldValueStyle);
            AddBankSalaryFooterField(sheet, currentRow + 2, checkedStart, "Employee Code", null, fieldLabelStyle, fieldValueStyle);
        }

        private static ICellStyle CreateFooterBorderStyle(IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.BorderTop = BorderStyle.Thin;

            return style;
        }

        private static void AddBankSalaryFooterField(
            ISheet sheet,
            int rowIndex,
            int columnIndex,
            string label,
            string? value,
            ICellStyle labelStyle,
            ICellStyle valueStyle)
        {
            var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);

            var labelCell = row.CreateCell(columnIndex);
            labelCell.SetCellValue(label);
            labelCell.CellStyle = labelStyle;

            if (!string.IsNullOrWhiteSpace(value))
            {
                var valueCell = row.CreateCell(columnIndex + 1);
                valueCell.SetCellValue(value);
                valueCell.CellStyle = valueStyle;
            }
        }

        private static ICellStyle CreateBankSalaryFooterStyle(IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;

            var font = workbook.CreateFont();
            font.IsBold = true;

            style.SetFont(font);

            return style;
        }

        private static ICellStyle CreateBankSalaryFooterValueStyle(IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;

            var font = workbook.CreateFont();
            font.IsBold = false;

            style.SetFont(font);

            return style;
        }

        // =============================================================
        // COLUMN WIDTHS (computed up front, not auto-sized at the end)
        // =============================================================

        private static void ApplyColumnWidths(ISheet sheet, DataTable dataTable)
        {
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                var maxLength = ToDisplayName(dataTable.Columns[i].ColumnName).Length;

                foreach (DataRow row in dataTable.Rows)
                {
                    var text = row[i]?.ToString() ?? string.Empty;

                    if (text.Length > maxLength)
                        maxLength = text.Length;
                }

                // padding + sane min/max bounds, in characters
                var widthChars = Math.Min(Math.Max(maxLength + 4, 10), 45);

                sheet.SetColumnWidth(i, widthChars * 256);
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