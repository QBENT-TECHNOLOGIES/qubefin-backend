using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Globalization;

namespace QubeFin.Payroll.Persistence.Repositories.ExcelHelpers
{
    public static class EmployeeSalaryExcelHelper
    {
        public static MemoryStream CreateEmployeeSalaryExcel(
            DataTable dataTable,
            byte[]? logoBytes,
            int month,
            int year)
        {
            var workbook = new XSSFWorkbook();

            try
            {
                var sheet = workbook.CreateSheet("Employee Salary");

                var currentRow = 0;

                // =========================================================
                // REMOVE UNWANTED COLUMNS
                // =========================================================

                var excludedColumns = new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    "PayMonthName",
                    "PayMonth",
                    "PayYear",
                    "EmployeeId"
                };

                var columns = dataTable.Columns
                    .Cast<DataColumn>()
                    .Where(x => !excludedColumns.Contains(x.ColumnName))
                    .ToList();

                // =========================================================
                // COLUMN WIDTHS
                // =========================================================

                ApplyColumnWidths(sheet, columns);

                // =========================================================
                // COMPANY LOGO / HEADER
                // =========================================================

                currentRow = ExcelCompanyHeaderHelper.AddCompanyLogo(
                    workbook,
                    sheet,
                    currentRow,
                    columns.Count,
                    logoBytes);

                // =========================================================
                // MAIN HEADER
                // =========================================================

                AddTitle(
                    workbook,
                    sheet,
                    ref currentRow,
                    "Employee Salary Register",
                    columns.Count);

                // =========================================================
                // SUB HEADER
                // =========================================================

                var monthName = GetPaymentMonth(month, year);

                AddSubHeader(
                    workbook,
                    sheet,
                    ref currentRow,
                    $"Salary Detail For the month of {monthName}",
                    columns.Count);

                // =========================================================
                // GROUP HEADER
                // =========================================================

                AddGroupedHeader(
                    workbook,
                    sheet,
                    ref currentRow,
                    columns);

                // =========================================================
                // COLUMN HEADER
                // =========================================================

                AddColumnHeader(
                    workbook,
                    sheet,
                    ref currentRow,
                    columns);

                // =========================================================
                // DATA
                // =========================================================

                var dataStartRow = currentRow;

                AddDataRows(
                    workbook,
                    sheet,
                    ref currentRow,
                    dataTable,
                    columns);

                // =========================================================
                // AUTO FILTER
                // =========================================================

                if (dataTable.Rows.Count > 0)
                {
                    sheet.SetAutoFilter(
                        new CellRangeAddress(
                            dataStartRow - 1,
                            currentRow - 1,
                            0,
                            columns.Count - 1));
                }

                // =========================================================
                // FREEZE HEADER
                // =========================================================

                sheet.CreateFreezePane(
                    0,
                    dataStartRow);

                // =========================================================
                // WRITE
                // =========================================================

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

        // =============================================================
        // TITLE
        // =============================================================

        private static void AddTitle(
            IWorkbook workbook,
            ISheet sheet,
            ref int currentRow,
            string title,
            int columnCount)
        {
            var row = sheet.CreateRow(currentRow++);

            row.HeightInPoints = 25;

            var cell = row.CreateCell(0);

            cell.SetCellValue(title);

            cell.CellStyle = CreateTitleStyle(workbook);

            for (var i = 1; i < columnCount; i++)
            {
                row.CreateCell(i).CellStyle =
                    CreateTitleStyle(workbook);
            }

            sheet.AddMergedRegion(
                new CellRangeAddress(
                    row.RowNum,
                    row.RowNum,
                    0,
                    columnCount - 1));
        }

        // =============================================================
        // SUB HEADER
        // =============================================================

        private static void AddSubHeader(
            IWorkbook workbook,
            ISheet sheet,
            ref int currentRow,
            string text,
            int columnCount)
        {
            var row = sheet.CreateRow(currentRow++);

            row.HeightInPoints = 20;

            var cell = row.CreateCell(0);

            cell.SetCellValue(text);

            cell.CellStyle = CreateSubHeaderStyle(workbook);

            for (var i = 1; i < columnCount; i++)
            {
                row.CreateCell(i).CellStyle =
                    CreateSubHeaderStyle(workbook);
            }

            sheet.AddMergedRegion(
                new CellRangeAddress(
                    row.RowNum,
                    row.RowNum,
                    0,
                    columnCount - 1));
        }

        // =============================================================
        // GROUP HEADER
        // =============================================================

        private static void AddGroupedHeader(
            IWorkbook workbook,
            ISheet sheet,
            ref int currentRow,
            List<DataColumn> columns)
        {
            var row = sheet.CreateRow(currentRow++);

            row.HeightInPoints = 20;

            var style = CreateGroupHeaderStyle(workbook);

            // ---------------------------------------------------------
            // Employee Detail
            // CompanyName
            // OrganizationName
            // EmployeeName
            // EmployeeCode
            // Designation
            // Grade
            // ---------------------------------------------------------

            MergeGroup(
                sheet,
                row,
                style,
                columns,
                "Employee Detail",
                "CompanyName",
                "BankAccountNo");

            // ---------------------------------------------------------
            // Earning
            // BASIC -> GROSS
            // ---------------------------------------------------------

            MergeGroup(
                sheet,
                row,
                style,
                columns,
                "Earning",
                "BASIC",
                "GROSS");

            // ---------------------------------------------------------
            // Deduction
            // PF -> LOPAMOUNT
            // ---------------------------------------------------------

            MergeGroup(
                sheet,
                row,
                style,
                columns,
                "Deduction",
                "PF",
                "LOPAMOUNT");

            // ---------------------------------------------------------
            // Employer Contribution
            // EPF -> GRATUITY
            // ---------------------------------------------------------

            MergeGroup(
                sheet,
                row,
                style,
                columns,
                "Employer Contribution",
                "EPF",
                "GRATUITY");

            MergeGroup(
                sheet,
                row,
                style,
                columns,
                "NETPAY",
                "NETPAY",
                "NETPAY");
        }

        // =============================================================
        // MERGE GROUP
        // =============================================================

        private static void MergeGroup(
        ISheet sheet,
        IRow row,
        ICellStyle style,
        List<DataColumn> columns,
        string title,
        string startColumn,
        string endColumn)
        {
            var startIndex = FindColumnIndex(columns, startColumn);
            var endIndex = FindColumnIndex(columns, endColumn);

            if (startIndex < 0 || endIndex < 0)
            {
                return;
            }

            for (var i = startIndex; i <= endIndex; i++)
            {
                var cell = row.GetCell(i) ?? row.CreateCell(i);
                cell.CellStyle = style;
            }

            row.GetCell(startIndex).SetCellValue(title);

            // Only merge when the group actually spans more than one column
            if (endIndex > startIndex)
            {
                sheet.AddMergedRegion(
                    new CellRangeAddress(row.RowNum, row.RowNum, startIndex, endIndex));
            }
        }

        // =============================================================
        // COLUMN HEADER
        // =============================================================

        private static void AddColumnHeader(
            IWorkbook workbook,
            ISheet sheet,
            ref int currentRow,
            List<DataColumn> columns)
        {
            var row = sheet.CreateRow(currentRow++);

            row.HeightInPoints = 22;

            var style = CreateColumnHeaderStyle(workbook);

            for (var i = 0; i < columns.Count; i++)
            {
                var cell = row.CreateCell(i);

                cell.SetCellValue(columns[i].ColumnName);

                cell.CellStyle = style;
            }
        }

        // =============================================================
        // DATA ROWS
        // =============================================================

        private static void AddDataRows(
            IWorkbook workbook,
            ISheet sheet,
            ref int currentRow,
            DataTable dataTable,
            List<DataColumn> columns)
        {
            var textStyle = CreateDataStyle(workbook);

            var numericStyle =
                CreateNumericDataStyle(workbook);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var row = sheet.CreateRow(currentRow++);

                for (var i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];

                    var cell = row.CreateCell(i);

                    var value = dataRow[column];

                    if (value == DBNull.Value || value == null)
                    {
                        cell.SetCellValue(string.Empty);

                        cell.CellStyle = textStyle;

                        continue;
                    }

                    if (IsNumericColumn(column))
                    {
                        if (decimal.TryParse(
                                value.ToString(),
                                NumberStyles.Any,
                                CultureInfo.InvariantCulture,
                                out var numericValue))
                        {
                            cell.SetCellValue(
                                (double)numericValue);

                            cell.CellStyle =
                                numericStyle;
                        }
                        else
                        {
                            cell.SetCellValue(
                                value.ToString());

                            cell.CellStyle =
                                textStyle;
                        }
                    }
                    else
                    {
                        cell.SetCellValue(
                            value.ToString());

                        cell.CellStyle =
                            textStyle;
                    }
                }
            }
        }

        // =============================================================
        // FIND COLUMN
        // =============================================================

        private static int FindColumnIndex(
            List<DataColumn> columns,
            string columnName)
        {
            for (var i = 0; i < columns.Count; i++)
            {
                if (columns[i].ColumnName.Equals(
                        columnName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        // =============================================================
        // COLUMN WIDTHS
        // =============================================================

        private static void ApplyColumnWidths(
            ISheet sheet,
            List<DataColumn> columns)
        {
            for (var i = 0; i < columns.Count; i++)
            {
                var name = columns[i].ColumnName;

                if (name.Equals(
                        "CompanyName",
                        StringComparison.OrdinalIgnoreCase))
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }
                else if (name.Equals(
                             "OrganizationName",
                             StringComparison.OrdinalIgnoreCase))
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }
                else if (name.Equals(
                             "EmployeeName",
                             StringComparison.OrdinalIgnoreCase))
                {
                    sheet.SetColumnWidth(i, 24 * 256);
                }
                else if (name.Equals(
                             "EmployeeCode",
                             StringComparison.OrdinalIgnoreCase))
                {
                    sheet.SetColumnWidth(i, 18 * 256);
                }
                else if (name.Equals(
                             "Designation",
                             StringComparison.OrdinalIgnoreCase))
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }
                else if (name.Equals(
                             "Grade",
                             StringComparison.OrdinalIgnoreCase))
                {
                    sheet.SetColumnWidth(i, 10 * 256);
                }
                else
                {
                    sheet.SetColumnWidth(i, 14 * 256);
                }
            }
        }

        // =============================================================
        // NUMERIC COLUMN
        // =============================================================

        private static bool IsNumericColumn(
            DataColumn column)
        {
            var type =
                Nullable.GetUnderlyingType(
                    column.DataType)
                ?? column.DataType;

            return type == typeof(byte)
                   || type == typeof(short)
                   || type == typeof(int)
                   || type == typeof(long)
                   || type == typeof(float)
                   || type == typeof(double)
                   || type == typeof(decimal);
        }

        // =============================================================
        // PAYMENT MONTH
        // =============================================================

        private static string GetPaymentMonth(
            int month,
            int year)
        {
            if (month < 1 || month > 12)
            {
                return string.Empty;
            }

            return new DateTime(
                    year,
                    month,
                    1)
                .ToString("MMMM yyyy");
        }

        // =============================================================
        // TITLE STYLE
        // =============================================================

        private static ICellStyle CreateTitleStyle(
            IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment =
                HorizontalAlignment.Center;

            style.VerticalAlignment =
                VerticalAlignment.Center;

            var font = workbook.CreateFont();

            font.IsBold = true;
            font.FontHeightInPoints = 16;

            style.SetFont(font);

            return style;
        }

        // =============================================================
        // SUB HEADER STYLE
        // =============================================================

        private static ICellStyle CreateSubHeaderStyle(
            IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment =
                HorizontalAlignment.Center;

            style.VerticalAlignment =
                VerticalAlignment.Center;

            var font = workbook.CreateFont();

            font.IsBold = true;
            font.FontHeightInPoints = 11;

            style.SetFont(font);

            return style;
        }

        // =============================================================
        // GROUP HEADER STYLE
        // =============================================================

        private static ICellStyle CreateGroupHeaderStyle(
            IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment =
                HorizontalAlignment.Center;

            style.VerticalAlignment =
                VerticalAlignment.Center;

            style.FillForegroundColor =
                IndexedColors.Grey25Percent.Index;

            style.FillPattern =
                FillPattern.SolidForeground;

            style.BorderTop =
                BorderStyle.Thin;

            style.BorderBottom =
                BorderStyle.Thin;

            style.BorderLeft =
                BorderStyle.Thin;

            style.BorderRight =
                BorderStyle.Thin;

            var font = workbook.CreateFont();

            font.IsBold = true;
            font.FontHeightInPoints = 11;

            style.SetFont(font);

            return style;
        }

        // =============================================================
        // COLUMN HEADER STYLE
        // =============================================================

        private static ICellStyle CreateColumnHeaderStyle(
            IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment =
                HorizontalAlignment.Center;

            style.VerticalAlignment =
                VerticalAlignment.Center;

            style.BorderTop =
                BorderStyle.Thin;

            style.BorderBottom =
                BorderStyle.Thin;

            style.BorderLeft =
                BorderStyle.Thin;

            style.BorderRight =
                BorderStyle.Thin;

            var font = workbook.CreateFont();

            font.IsBold = true;
            font.FontHeightInPoints = 10;

            style.SetFont(font);

            return style;
        }

        // =============================================================
        // DATA STYLE
        // =============================================================

        private static ICellStyle CreateDataStyle(
            IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment =
                HorizontalAlignment.Left;

            style.VerticalAlignment =
                VerticalAlignment.Center;

            style.BorderTop =
                BorderStyle.Thin;

            style.BorderBottom =
                BorderStyle.Thin;

            style.BorderLeft =
                BorderStyle.Thin;

            style.BorderRight =
                BorderStyle.Thin;

            return style;
        }

        // =============================================================
        // NUMERIC DATA STYLE
        // =============================================================

        private static ICellStyle CreateNumericDataStyle(
            IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.Alignment =
                HorizontalAlignment.Right;

            style.VerticalAlignment =
                VerticalAlignment.Center;

            style.BorderTop =
                BorderStyle.Thin;

            style.BorderBottom =
                BorderStyle.Thin;

            style.BorderLeft =
                BorderStyle.Thin;

            style.BorderRight =
                BorderStyle.Thin;

            return style;
        }
    }
}