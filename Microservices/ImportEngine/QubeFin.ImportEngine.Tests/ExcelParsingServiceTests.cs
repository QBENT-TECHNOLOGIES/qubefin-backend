using QubeFin.ImportEngine.Application.Imports.Models;
using QubeFin.ImportEngine.Application.Imports.Services;
using Xunit;

namespace QubeFin.ImportEngine.Tests;

public class ExcelParsingServiceTests
{
    private readonly ExcelParsingService _sut = new();

    private static List<ImportColumnDefinition> PolicyColumns() =>
    [
        new() { Header = "Policy No", Param = "PolicyNo", Type = ImportColumnType.String },
        new() { Header = "Premium Amount", Param = "PremiumAmount", Type = ImportColumnType.Number },
        new() { Header = "Policy Date", Param = "PolicyDate", Type = ImportColumnType.Date },
    ];

    [Fact]
    public void HeaderMatching_IsCaseInsensitiveAndTrimsWhitespace()
    {
        var headers = new[] { "  policy   no ", " PREMIUM amount", "Policy Date" };
        var rows = new List<object?[]> { new object?[] { "P1", "100", "01-01-2026" } };

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.Empty(result.MissingHeaders);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void MissingHeader_ReturnsClearMessageListingAllMissingColumns()
    {
        var headers = new[] { "Policy No" };
        var rows = new List<object?[]>();

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.Equal(["Premium Amount", "Policy Date"], result.MissingHeaders);
    }

    [Theory]
    [InlineData("1500.50")]
    [InlineData("1,500.50")]
    public void NumberColumn_ParsesUsingInvariantCulture(string value)
    {
        var headers = new[] { "Policy No", "Premium Amount", "Policy Date" };
        var rows = new List<object?[]> { new object?[] { "P1", value, "01-01-2026" } };

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.Empty(result.Errors);
        Assert.Equal("1500.5", result.Rows[0]["PremiumAmount"]);
    }

    [Fact]
    public void NumberColumn_InvalidValue_ProducesRowError()
    {
        var headers = new[] { "Policy No", "Premium Amount", "Policy Date" };
        var rows = new List<object?[]> { new object?[] { "P1", "not-a-number", "01-01-2026" } };

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.Single(result.Errors);
        Assert.Equal("Premium Amount", result.Errors[0].Column);
    }

    [Theory]
    [InlineData("31-01-2026")]
    [InlineData("31/01/2026")]
    public void DateColumn_AcceptsExactTextFormats(string value)
    {
        var headers = new[] { "Policy No", "Premium Amount", "Policy Date" };
        var rows = new List<object?[]> { new object?[] { "P1", "100", value } };

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.Empty(result.Errors);
        Assert.Equal("2026-01-31", result.Rows[0]["PolicyDate"]);
    }

    [Fact]
    public void DateColumn_AcceptsExcelSerialDate()
    {
        var headers = new[] { "Policy No", "Premium Amount", "Policy Date" };
        var serial = DateTime.FromOADate(0).AddDays(46023).ToOADate();
        var rows = new List<object?[]> { new object?[] { "P1", "100", serial.ToString() } };

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.Empty(result.Errors);
    }

    [Fact]
    public void DateColumn_AcceptsRealDateTimeCell()
    {
        var headers = new[] { "Policy No", "Premium Amount", "Policy Date" };
        var rows = new List<object?[]> { new object?[] { "P1", "100", new DateTime(2026, 5, 16) } };

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.Empty(result.Errors);
        Assert.Equal("2026-05-16", result.Rows[0]["PolicyDate"]);
    }

    [Theory]
    [InlineData("31-02-2026")]
    [InlineData("2026-05-16")]
    public void DateColumn_RejectsInvalidOrNonExactFormats(string value)
    {
        var headers = new[] { "Policy No", "Premium Amount", "Policy Date" };
        var rows = new List<object?[]> { new object?[] { "P1", "100", value } };

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.Single(result.Errors);
        Assert.Equal("Policy Date", result.Errors[0].Column);
    }

    [Fact]
    public void EmptyOrWhitespaceCell_MapsToNull()
    {
        var headers = new[] { "Policy No", "Premium Amount", "Policy Date" };
        var rows = new List<object?[]> { new object?[] { "P1", "   ", "01-01-2026" } };

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.Empty(result.Errors);
        Assert.Null(result.Rows[0]["PremiumAmount"]);
    }

    [Fact]
    public void FullyEmptyRow_IsSkipped()
    {
        var headers = new[] { "Policy No", "Premium Amount", "Policy Date" };
        var rows = new List<object?[]> { new object?[] { null, "", "  " }, new object?[] { "P1", "100", "01-01-2026" } };

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.Single(result.Rows);
        Assert.Equal(1, result.TotalRows);
    }

    [Fact]
    public void AnyRowError_RejectsWholeFile()
    {
        var headers = new[] { "Policy No", "Premium Amount", "Policy Date" };
        var rows = new List<object?[]>
        {
            new object?[] { "P1", "100", "01-01-2026" },
            new object?[] { "P2", "bad-number", "01-01-2026" },
        };

        var result = _sut.ValidateAndMapRows(headers, rows, PolicyColumns(), maxRows: 100);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }
}
