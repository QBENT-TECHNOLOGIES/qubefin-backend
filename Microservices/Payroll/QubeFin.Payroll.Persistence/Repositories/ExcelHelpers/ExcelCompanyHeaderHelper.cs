using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace QubeFin.Payroll.Persistence.Repositories.ExcelHelpers;

public static class ExcelCompanyHeaderHelper
{
    public static int AddCompanyHeader(
        IWorkbook workbook,
        ISheet sheet,
        int currentRow,
        int columnCount,
        byte[]? logoBytes)
    {
        if (columnCount <= 0)
            columnCount = 1;

        var companyStyle = CreateCompanyStyle(workbook);
        var companyNameStyle = CreateCompanyNameStyle(workbook);
        var contactStyle = CreateContactStyle(workbook);

        // ---------------------------------------------------------
        // Logo
        // ---------------------------------------------------------

        if (logoBytes is not null && logoBytes.Length > 0)
        {
            AddLogo(
                workbook,
                sheet,
                currentRow,
                columnCount,
                logoBytes);

            currentRow += 4;
        }
        else
        {
            currentRow++;
        }

        // ---------------------------------------------------------
        // Company Name
        // ---------------------------------------------------------

        var companyRow = sheet.CreateRow(currentRow++);
        companyRow.HeightInPoints = 22;

        var companyCell = companyRow.CreateCell(0);

        companyCell.SetCellValue(
            "WeGrow Financial Services Private Limited");

        companyCell.CellStyle = companyNameStyle;

        Merge(
            sheet,
            companyRow.RowNum,
            columnCount);

        // ---------------------------------------------------------
        // Address
        // ---------------------------------------------------------

        var addressRow = sheet.CreateRow(currentRow++);
        addressRow.HeightInPoints = 20;

        var addressCell = addressRow.CreateCell(0);

        addressCell.SetCellValue(
            "AE -592, Salt Lake City, Sector – I, Kolkata, West Bengal");

        addressCell.CellStyle = companyStyle;

        Merge(
            sheet,
            addressRow.RowNum,
            columnCount);



        // ---------------------------------------------------------
        // Phone + Email
        // ---------------------------------------------------------

        var contactRow = sheet.CreateRow(currentRow++);
        contactRow.HeightInPoints = 20;

        var middleColumn = columnCount / 2;

        // Phone - left half
        var phoneCell = contactRow.CreateCell(0);

        phoneCell.SetCellValue(
            "Phone: +91 9836036541");

        phoneCell.CellStyle = contactStyle;

        if (middleColumn > 0)
        {
            sheet.AddMergedRegion(
                new CellRangeAddress(
                    contactRow.RowNum,
                    contactRow.RowNum,
                    0,
                    middleColumn - 1));
        }

        // Email - right half
        var emailCell = contactRow.CreateCell(middleColumn);

        emailCell.SetCellValue(
            "Email ID: hr@wegrowindia.com");

        emailCell.CellStyle = contactStyle;

        if (columnCount - 1 >= middleColumn)
        {
            sheet.AddMergedRegion(
                new CellRangeAddress(
                    contactRow.RowNum,
                    contactRow.RowNum,
                    middleColumn,
                    columnCount - 1));
        }

        // ---------------------------------------------------------
        // Separator
        // ---------------------------------------------------------

        var separatorRow = sheet.CreateRow(currentRow++);
        separatorRow.HeightInPoints = 8;

        var separatorCell = separatorRow.CreateCell(0);

        separatorCell.SetCellValue(
            "________________________________________________________________________");

        separatorCell.CellStyle = CreateSeparatorStyle(workbook);

        Merge(
            sheet,
            separatorRow.RowNum,
            columnCount);

        // Small spacing
        currentRow++;

        return currentRow;
    }

    // =============================================================
    // LOGO
    // =============================================================

    private static void AddLogo(
        IWorkbook workbook,
        ISheet sheet,
        int currentRow,
        int columnCount,
        byte[] logoBytes)
    {
        var pictureType = DetectPictureType(logoBytes);

        var pictureIndex = workbook.AddPicture(
            logoBytes,
            pictureType);

        var drawing = sheet.CreateDrawingPatriarch();

        // Center logo horizontally
        var logoStartColumn = Math.Max(
            (columnCount - 4) / 2,
            0);

        var logoEndColumn = Math.Min(
            logoStartColumn + 4,
            columnCount);

        var anchor = drawing.CreateAnchor(
            0,
            0,
            0,
            0,
            logoStartColumn,
            currentRow,
            logoEndColumn,
            currentRow + 4);

        drawing.CreatePicture(
            anchor,
            pictureIndex);
    }

    // =============================================================
    // STYLES
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
    // HELPERS
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
