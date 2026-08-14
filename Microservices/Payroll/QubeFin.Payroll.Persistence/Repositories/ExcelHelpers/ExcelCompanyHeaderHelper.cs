using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace QubeFin.Payroll.Persistence.Repositories.ExcelHelpers;

public static class ExcelCompanyHeaderHelper
{
    public static int AddCompanyHeader(IWorkbook workbook, ISheet sheet, int currentRow, int columnCount, byte[]? logoBytes)
    {
        if (columnCount <= 0) columnCount = 1;

        var companyStyle = CreateCompanyStyle(workbook);
        var companyNameStyle = CreateCompanyNameStyle(workbook);
        var contactStyle = CreateContactStyle(workbook);
        var separatorStyle = CreateSeparatorStyle(workbook);

        // =========================================================
        // LOGO
        // =========================================================

        if (logoBytes is not null && logoBytes.Length > 0)
        {
            AddLogo(workbook, sheet, currentRow, columnCount, logoBytes);

            // Make sure the rows exist, then merge the whole 4x(columnCount)
            // block the logo sits on into one region — merges both across
            // rows and across columns, matching the rest of the header.
            for (var i = 0; i < 4; i++)
            {
                if (sheet.GetRow(currentRow + i) is null)
                {
                    sheet.CreateRow(currentRow + i);
                }
            }

            MergeBlock(sheet, currentRow, currentRow + 3, columnCount);

            currentRow += 4;
        }
        else
        {
            currentRow++;
        }

        // =========================================================
        // COMPANY NAME
        // =========================================================

        var companyRow = sheet.CreateRow(currentRow++);
        companyRow.HeightInPoints = 22;

        var companyCell = companyRow.CreateCell(0);
        companyCell.SetCellValue("WeGrow Financial Services Private Limited");
        companyCell.CellStyle = companyNameStyle;

        Merge(sheet, companyRow.RowNum, columnCount);

        // =========================================================
        // ADDRESS
        // =========================================================

        var addressRow = sheet.CreateRow(currentRow++);
        addressRow.HeightInPoints = 20;

        var addressCell = addressRow.CreateCell(0);
        addressCell.SetCellValue("AE -592, Salt Lake City, Sector – I, Kolkata, West Bengal");
        addressCell.CellStyle = companyStyle;

        Merge(sheet, addressRow.RowNum, columnCount);

        // =========================================================
        // PHONE + EMAIL
        // =========================================================

        var contactRow = sheet.CreateRow(currentRow++);
        contactRow.HeightInPoints = 20;

        var contactCell = contactRow.CreateCell(0);
        contactCell.SetCellValue("Phone: +91 9836036541       Email ID: hr@wegrowindia.com");
        contactCell.CellStyle = contactStyle;

        // Merge entire row
        Merge(sheet, contactRow.RowNum, columnCount);

        // =========================================================
        // SEPARATOR
        // =========================================================

        var separatorRow = sheet.CreateRow(currentRow++);
        separatorRow.HeightInPoints = 20; // was 8 — now matches the other header rows
        var separatorCell = separatorRow.CreateCell(0);
        separatorCell.SetCellValue("");
        separatorCell.CellStyle = separatorStyle;
        Merge(sheet, separatorRow.RowNum, columnCount);

        return currentRow;
    }
    private const double EmuPerPixel = 9525.0;      // 914400 EMU/inch ÷ 96 DPI
    private const double DefaultCharWidthPx = 7.0;  // ~digit width for Calibri 11 / Arial 10

    private static void AddLogo(IWorkbook workbook, ISheet sheet, int currentRow, int columnCount, byte[] logoBytes)
    {
        var pictureType = DetectPictureType(logoBytes);
        var pictureIndex = workbook.AddPicture(logoBytes, pictureType);
        var drawing = sheet.CreateDrawingPatriarch();

        // Real pixel width of every column — columns are rarely equal,
        // so centering has to use actual widths, not just column count.
        var columnPixelWidths = new double[columnCount];
        double totalWidthPx = 0;

        for (var i = 0; i < columnCount; i++)
        {
            var widthInChars = sheet.GetColumnWidth(i) / 256.0;
            columnPixelWidths[i] = widthInChars * DefaultCharWidthPx;
            totalWidthPx += columnPixelWidths[i];
        }

        // Keep the same "roughly 4 columns wide" footprint as before,
        // just sized from the real average column width instead of a
        // raw column-count guess.
        var logoWidthPx = columnCount > 0 ? Math.Min(totalWidthPx, (totalWidthPx / columnCount) * 2) : totalWidthPx;

        var offsetXPx = Math.Max((totalWidthPx - logoWidthPx) / 2.0, 0);

        var (col1, dx1) = ResolveColumnOffset(columnPixelWidths, offsetXPx);
        var (col2, dx2) = ResolveColumnOffset(columnPixelWidths, offsetXPx + logoWidthPx);

        var anchor = drawing.CreateAnchor((int)Math.Round(dx1 * EmuPerPixel), 0, (int)Math.Round(dx2 * EmuPerPixel), 0, col1, currentRow, col2, currentRow + 4);

        drawing.CreatePicture(anchor, pictureIndex);
    }

    /// <summary>
    /// Given a target pixel offset from the left edge of the sheet,
    /// returns which column it falls in and how far into that column
    /// it is — Excel anchors are expressed as (column, offset-in-column),
    /// not raw pixel coordinates.
    /// </summary>
    private static (int column, double offsetInColumnPx) ResolveColumnOffset(double[] columnPixelWidths, double targetOffsetPx)
    {
        double cumulative = 0;

        for (var i = 0; i < columnPixelWidths.Length; i++)
        {
            var next = cumulative + columnPixelWidths[i];

            if (targetOffsetPx <= next || i == columnPixelWidths.Length - 1)
            {
                var offsetInColumn = Math.Max(targetOffsetPx - cumulative, 0);
                offsetInColumn = Math.Min(offsetInColumn, columnPixelWidths[i]);
                return (i, offsetInColumn);
            }

            cumulative = next;
        }

        return (columnPixelWidths.Length - 1, 0);
    }

    // =============================================================
    // COMPANY NAME STYLE
    // =============================================================

    private static ICellStyle CreateCompanyNameStyle(
        IWorkbook workbook)
    {
        var style = workbook.CreateCellStyle();

        style.Alignment = HorizontalAlignment.Center;
        style.VerticalAlignment = VerticalAlignment.Center;

        var font = workbook.CreateFont();

        font.FontName = "Arial";
        font.FontHeightInPoints = 14;
        font.IsBold = true;

        style.SetFont(font);

        return style;
    }

    // =============================================================
    // COMPANY STYLE
    // =============================================================

    private static ICellStyle CreateCompanyStyle(
        IWorkbook workbook)
    {
        var style = workbook.CreateCellStyle();

        style.Alignment = HorizontalAlignment.Center;
        style.VerticalAlignment = VerticalAlignment.Center;

        var font = workbook.CreateFont();

        font.FontName = "Arial";
        font.FontHeightInPoints = 10;

        style.SetFont(font);

        return style;
    }

    // =============================================================
    // CONTACT STYLE
    // =============================================================

    private static ICellStyle CreateContactStyle(
        IWorkbook workbook)
    {
        var style = workbook.CreateCellStyle();

        style.Alignment = HorizontalAlignment.Center;
        style.VerticalAlignment = VerticalAlignment.Center;

        var font = workbook.CreateFont();

        font.FontName = "Arial";
        font.FontHeightInPoints = 10;
        font.IsBold = true;

        style.SetFont(font);

        return style;
    }

    // =============================================================
    // SEPARATOR STYLE
    // =============================================================

    private static ICellStyle CreateSeparatorStyle(
        IWorkbook workbook)
    {
        var style = workbook.CreateCellStyle();

        style.Alignment = HorizontalAlignment.Center;
        style.VerticalAlignment = VerticalAlignment.Center;

        var font = workbook.CreateFont();

        font.FontName = "Arial";
        font.FontHeightInPoints = 8;

        style.SetFont(font);

        return style;
    }

    // =============================================================
    // MERGE
    // =============================================================

    private static void Merge(
        ISheet sheet,
        int row,
        int columnCount)
    {
        if (columnCount <= 1)
            return;

        sheet.AddMergedRegion(
            new CellRangeAddress(
                row,
                row,
                0,
                columnCount - 1));
    }

    // =============================================================
    // MERGE (multi-row block)
    // =============================================================

    private static void MergeBlock(
        ISheet sheet,
        int firstRow,
        int lastRow,
        int columnCount)
    {
        if (columnCount <= 1 && firstRow == lastRow)
            return;

        sheet.AddMergedRegion(
            new CellRangeAddress(
                firstRow,
                lastRow,
                0,
                Math.Max(columnCount - 1, 0)));
    }

    // =============================================================
    // IMAGE TYPE
    // =============================================================

    private static PictureType DetectPictureType(
        byte[] imageBytes)
    {
        // PNG
        if (imageBytes.Length >= 4 &&
            imageBytes[0] == 0x89 &&
            imageBytes[1] == 0x50 &&
            imageBytes[2] == 0x4E &&
            imageBytes[3] == 0x47)
        {
            return PictureType.PNG;
        }

        // JPEG
        if (imageBytes.Length >= 3 &&
            imageBytes[0] == 0xFF &&
            imageBytes[1] == 0xD8 &&
            imageBytes[2] == 0xFF)
        {
            return PictureType.JPEG;
        }

        // GIF
        if (imageBytes.Length >= 4 &&
            imageBytes[0] == 0x47 &&
            imageBytes[1] == 0x49 &&
            imageBytes[2] == 0x46 &&
            imageBytes[3] == 0x38)
        {
            return PictureType.GIF;
        }

        return PictureType.PNG;
    }
}