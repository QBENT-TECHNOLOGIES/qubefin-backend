using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Data;

namespace QubeFin.Report.Persistence.Repositories.ExcelHelpers.CustomHelper
{
    public static class BankSalaryDisbursementExcelHelper
    {
        public static MemoryStream CreateBankSalaryDisbursementExcel(DataTable dataTable, byte[]? logoBytes, int month, int year, string name, string designation, string code)
        {
            var workbook = new XSSFWorkbook();

            try
            {
                var sheet = workbook.CreateSheet("Report");

                // Same existing column width logic
                ExcelReportHelper.ApplyColumnWidths(sheet, dataTable);

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

                ExcelReportHelper.AddHeader(
                    workbook,
                    sheet,
                    ref currentRow,
                    "Bank Salary Disbursement Sheet",
                    dataTable.Columns.Count);

                // =========================================================
                // SUB HEADER - SAME AS EXISTING
                // =========================================================

                var monthName = GetPaymentMonth(month, year);

                ExcelReportHelper.AddSubHeader(
                    workbook,
                    sheet,
                    ref currentRow,
                    $"For the month of {monthName}",
                    dataTable.Columns.Count);

                // =========================================================
                // COLUMN HEADERS - SAME AS EXISTING
                // =========================================================

                ExcelReportHelper.AddColumnHeaders(
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
                    ExcelReportHelper.AddNoDataRow(workbook, sheet, ref currentRow, dataTable.Columns.Count);
                }
                else
                {
                    ExcelReportHelper.AddData(workbook, sheet, ref currentRow, dataTable);

                    AddBankSalaryTotal(workbook, sheet, ref currentRow, dataTable);
                }

                // Footer should always be shown
                AddBankSalaryFooter(workbook, sheet, ref currentRow, dataTable.Columns.Count, name, designation, code);

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
        private static void AddBankSalaryTotal(IWorkbook workbook, ISheet sheet, ref int currentRow, DataTable dataTable)
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
        private static void AddBankSalaryFooter(IWorkbook workbook, ISheet sheet, ref int currentRow, int columnCount, string preparedByName, string preparedByDesignation, string preparedByCode)
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
        private static void AddBankSalaryFooterField(ISheet sheet, int rowIndex, int columnIndex, string label, string? value, ICellStyle labelStyle, ICellStyle valueStyle)
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
    }
}
