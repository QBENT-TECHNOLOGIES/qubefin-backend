using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace QubeFin.Report.Persistence.Repositories.ExcelHelpers
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

        #region GENERIC (LINQ) OVERLOAD

        public static MemoryStream CreateExcel<T>(IEnumerable<T> data, ExcelReportOptions options, byte[]? logoBytes)
        {
            // Materialize ONCE — avoids re-running a deferred LINQ/EF query twice
            // (once for column widths, once for writing rows).
            var rows = data as IReadOnlyList<T> ?? data.ToList();

            var workbook = new XSSFWorkbook();

            try
            {
                var sheet = workbook.CreateSheet("Report");
                var accessors = PropertyAccessorCache<T>.Accessors;

                ApplyColumnWidths(sheet, accessors, rows);

                var currentRow = 0;

                if (options.ShowCompanyHeader)
                    currentRow = ExcelCompanyHeaderHelper.AddCompanyHeader(workbook, sheet, currentRow, accessors.Length, logoBytes);

                if (!string.IsNullOrWhiteSpace(options.ReportTitle))
                    AddHeader(workbook, sheet, ref currentRow, options.ReportTitle, accessors.Length);

                if (!string.IsNullOrWhiteSpace(options.SubHeader))
                    AddSubHeader(workbook, sheet, ref currentRow, options.SubHeader, accessors.Length);

                AddColumnHeaders(workbook, sheet, ref currentRow, accessors);

                if (rows.Count == 0)
                    AddNoDataRow(workbook, sheet, ref currentRow, accessors.Length);
                else
                    AddData(workbook, sheet, ref currentRow, accessors, rows);

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

        private static void ApplyColumnWidths<T>(ISheet sheet, (string Name, Func<T, object?> Getter)[] accessors, IReadOnlyList<T> rows)
        {
            for (var i = 0; i < accessors.Length; i++)
            {
                var maxLength = ToDisplayName(accessors[i].Name).Length;

                foreach (var item in rows)
                {
                    var text = accessors[i].Getter(item)?.ToString() ?? string.Empty;
                    if (text.Length > maxLength)
                        maxLength = text.Length;
                }

                var widthChars = Math.Min(Math.Max(maxLength + 4, 10), 45);
                sheet.SetColumnWidth(i, widthChars * 256);
            }
        }

        private static void AddColumnHeaders<T>(IWorkbook workbook, ISheet sheet, ref int currentRow, (string Name, Func<T, object?> Getter)[] accessors)
        {
            var row = sheet.CreateRow(currentRow++);
            var style = CreateColumnHeaderStyle(workbook);

            for (var i = 0; i < accessors.Length; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(ToDisplayName(accessors[i].Name));
                cell.CellStyle = style;
            }
        }

        private static void AddData<T>(IWorkbook workbook, ISheet sheet, ref int currentRow, (string Name, Func<T, object?> Getter)[] accessors, IReadOnlyList<T> rows)
        {
            var dateStyle = workbook.CreateCellStyle();
            dateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy");

            foreach (var item in rows)
            {
                var row = sheet.CreateRow(currentRow++);

                for (var i = 0; i < accessors.Length; i++)
                {
                    var cell = row.CreateCell(i);
                    var value = accessors[i].Getter(item);
                    ExcelCellHelper.SetValue(cell, value ?? DBNull.Value, dateStyle);
                }
            }
        }

        private static class PropertyAccessorCache<T>
        {
            public static readonly (string Name, Func<T, object?> Getter)[] Accessors =
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p =>
                    {
                        var instance = Expression.Parameter(typeof(T), "x");
                        var property = Expression.Property(instance, p);
                        var convert = Expression.Convert(property, typeof(object));
                        var getter = Expression.Lambda<Func<T, object?>>(convert, instance).Compile();
                        return (p.Name, Getter: getter);
                    })
                    .ToArray();
        }

        #region OBJECT (RUNTIME-TYPE) OVERLOAD — for IEnumerable<object> callers

        public static MemoryStream CreateExcel(IEnumerable<object> data, ExcelReportOptions options, byte[]? logoBytes)
        {
            var rows = data as IReadOnlyList<object> ?? data.ToList();

            // Resolve the REAL type from the first item, not from the object wrapper.
            // Falls back to object if the collection is empty (no columns to render).
            var itemType = rows.Count > 0 ? rows[0]!.GetType() : typeof(object);

            var workbook = new XSSFWorkbook();

            try
            {
                var sheet = workbook.CreateSheet("Report");
                var accessors = ObjectPropertyAccessorCache.GetAccessors(itemType);

                ApplyColumnWidths(sheet, accessors, rows);

                var currentRow = 0;

                if (options.ShowCompanyHeader)
                    currentRow = ExcelCompanyHeaderHelper.AddCompanyHeader(workbook, sheet, currentRow, accessors.Length, logoBytes);
                else 
                    currentRow = ExcelCompanyHeaderHelper.AddCompanyLogo(workbook, sheet, currentRow, accessors.Length, logoBytes);

                if (!string.IsNullOrWhiteSpace(options.ReportTitle))
                    AddHeader(workbook, sheet, ref currentRow, options.ReportTitle, accessors.Length);

                if (!string.IsNullOrWhiteSpace(options.SubHeader))
                    AddSubHeader(workbook, sheet, ref currentRow, options.SubHeader, accessors.Length);

                AddColumnHeaders(workbook, sheet, ref currentRow, accessors);

                if (rows.Count == 0)
                    AddNoDataRow(workbook, sheet, ref currentRow, accessors.Length);
                else
                    AddData(workbook, sheet, ref currentRow, accessors, rows);

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

        private static void ApplyColumnWidths(ISheet sheet, (string Name, Func<object, object?> Getter)[] accessors, IReadOnlyList<object> rows)
        {
            for (var i = 0; i < accessors.Length; i++)
            {
                var maxLength = ToDisplayName(accessors[i].Name).Length;

                foreach (var item in rows)
                {
                    var text = accessors[i].Getter(item)?.ToString() ?? string.Empty;
                    if (text.Length > maxLength)
                        maxLength = text.Length;
                }

                var widthChars = Math.Min(Math.Max(maxLength + 4, 10), 45);
                sheet.SetColumnWidth(i, widthChars * 256);
            }
        }

        private static void AddColumnHeaders(IWorkbook workbook, ISheet sheet, ref int currentRow, (string Name, Func<object, object?> Getter)[] accessors)
        {
            var row = sheet.CreateRow(currentRow++);
            var style = CreateColumnHeaderStyle(workbook);

            for (var i = 0; i < accessors.Length; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(ToDisplayName(accessors[i].Name));
                cell.CellStyle = style;
            }
        }

        private static void AddData(IWorkbook workbook, ISheet sheet, ref int currentRow, (string Name, Func<object, object?> Getter)[] accessors, IReadOnlyList<object> rows)
        {
            var dateStyle = workbook.CreateCellStyle();
            dateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy");

            foreach (var item in rows)
            {
                var row = sheet.CreateRow(currentRow++);

                for (var i = 0; i < accessors.Length; i++)
                {
                    var cell = row.CreateCell(i);
                    var value = accessors[i].Getter(item);
                    ExcelCellHelper.SetValue(cell, value ?? DBNull.Value, dateStyle);
                }
            }
        }

        private static class ObjectPropertyAccessorCache
        {
            private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, (string Name, Func<object, object?> Getter)[]> _cache = new();

            public static (string Name, Func<object, object?> Getter)[] GetAccessors(Type type) =>
                _cache.GetOrAdd(type, t =>
                    t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(p => (p.Name, Getter: (Func<object, object?>)p.GetValue))
                        .ToArray());
        }

        #endregion

        #endregion

        #region HELPERS
        public static void ApplyColumnWidths(ISheet sheet, DataTable dataTable)
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
        public static void AddHeader(IWorkbook workbook, ISheet sheet, ref int currentRow, string header, int columnCount)
        {
            var row = sheet.CreateRow(currentRow++);
            row.HeightInPoints = 25;

            var cell = row.CreateCell(0);
            cell.SetCellValue(header);
            cell.CellStyle = CreateHeaderStyle(workbook);

            MergeCells(sheet, row.RowNum, columnCount);
        }
        public static void AddSubHeader(IWorkbook workbook, ISheet sheet, ref int currentRow, string subHeader, int columnCount)
        {
            var row = sheet.CreateRow(currentRow++);
            row.HeightInPoints = 20;

            var cell = row.CreateCell(0);
            cell.SetCellValue(subHeader);
            cell.CellStyle = CreateSubHeaderStyle(workbook);

            MergeCells(sheet, row.RowNum, columnCount);
        }
        public static void AddColumnHeaders(IWorkbook workbook, ISheet sheet, ref int currentRow, DataTable dataTable)
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
        public static void AddData(IWorkbook workbook, ISheet sheet, ref int currentRow, DataTable dataTable)
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
        public static void AddNoDataRow(IWorkbook workbook, ISheet sheet, ref int currentRow, int columnCount)
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
        public static void MergeCells(ISheet sheet, int row, int columnCount)
        {
            if (columnCount > 1)
            {
                sheet.AddMergedRegion(new CellRangeAddress(row, row, 0, columnCount - 1));
            }
        }
        public static ICellStyle CreateHeaderStyle(IWorkbook workbook)
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
        public static ICellStyle CreateSubHeaderStyle(IWorkbook workbook)
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
        public static ICellStyle CreateColumnHeaderStyle(IWorkbook workbook)
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
        #endregion
    }
}