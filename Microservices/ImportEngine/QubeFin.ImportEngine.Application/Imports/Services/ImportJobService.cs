using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using QubeFin.ImportEngine.Application.Imports.Models;
using QubeFin.ImportEngine.Persistence.Repositories;
using System.Text.Json;

namespace QubeFin.ImportEngine.Application.Imports.Services;

public class ImportJobService(
    IImportTypeRepository importTypeRepository,
    IImportJobRepository importJobRepository,
    IImportDataRepository importDataRepository,
    IFileStorageRepository fileStorageRepository,
    IExcelParsingService excelParsingService,
    IOptions<ImportSettings> settings) : IImportJobService
{
    public async Task<Guid> CreateJobAsync(IFormFile file, Guid importTypeId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            throw new InvalidOperationException("The uploaded file is empty.");
        }

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only .xlsx files are supported.");
        }

        if (file.Length > settings.Value.MaxFileSizeBytes)
        {
            throw new InvalidOperationException($"File exceeds the maximum allowed size of {settings.Value.MaxFileSizeBytes} bytes.");
        }

        var importType = await importTypeRepository.GetByIdAsync(importTypeId, cancellationToken)
            ?? throw new InvalidOperationException("Import type not found.");

        if (!importType.IsActive)
        {
            throw new InvalidOperationException("Import type is not active.");
        }

        await using var stream = file.OpenReadStream();
        var storedFilePath = await fileStorageRepository.UploadFileAsync(stream, file.FileName, file.ContentType, cancellationToken, "ImportEngine");

        return await importJobRepository.CreateAsync(importTypeId, file.FileName, storedFilePath, userId, cancellationToken);
    }

    public async Task<ImportJobResult> ProcessJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await importJobRepository.GetByIdAsync(jobId, cancellationToken)
            ?? throw new InvalidOperationException("Import job not found.");

        var importType = await importTypeRepository.GetByIdAsync(job.ImportTypeId, cancellationToken)
            ?? throw new InvalidOperationException("Import type not found.");

        List<ImportColumnDefinition> columns;
        try
        {
            columns = JsonSerializer.Deserialize<List<ImportColumnDefinition>>(importType.Columns) ?? [];
        }
        catch (JsonException ex)
        {
            return await FailAsync(jobId, $"Import type configuration is invalid: {ex.Message}", cancellationToken);
        }

        await importJobRepository.SetProcessingAsync(jobId, cancellationToken);

        var fileUrl = await fileStorageRepository.GetFileUrlAsync("ImportEngine/" + job.StoredFilePath, cancellationToken);
        using var httpClient = new HttpClient();
        await using var fileStream = await httpClient.GetStreamAsync(fileUrl, cancellationToken);
        await using var bufferedStream = new MemoryStream();
        await fileStream.CopyToAsync(bufferedStream, cancellationToken);
        bufferedStream.Position = 0;

        var parseResult = excelParsingService.ParseWorkbook(bufferedStream, columns, settings.Value.MaxRows);

        if (parseResult.MissingHeaders.Count > 0)
        {
            var message = $"Missing required column(s): {string.Join(", ", parseResult.MissingHeaders)}";
            return await FailAsync(jobId, message, cancellationToken);
        }

        if (parseResult.Errors.Count > 0)
        {
            var errorJson = JsonSerializer.Serialize(parseResult.Errors);
            await importJobRepository.CompleteAsync(jobId, "Failed", parseResult.TotalRows, 0, parseResult.Errors.Count, errorJson, cancellationToken);
            return new ImportJobResult
            {
                JobId = jobId,
                Status = "Failed",
                TotalRows = parseResult.TotalRows,
                SuccessRows = 0,
                FailedRows = parseResult.Errors.Count,
                Errors = parseResult.Errors
            };
        }

        var dataJson = JsonSerializer.Serialize(parseResult.Rows);
        await importDataRepository.ExecuteImportSpAsync(importType.SpName, dataJson, cancellationToken);

        await importJobRepository.CompleteAsync(jobId, "Completed", parseResult.TotalRows, parseResult.TotalRows, 0, null, cancellationToken);

        return new ImportJobResult
        {
            JobId = jobId,
            Status = "Completed",
            TotalRows = parseResult.TotalRows,
            SuccessRows = parseResult.TotalRows,
            FailedRows = 0
        };
    }

    public async Task<ImportJobResult?> GetJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await importJobRepository.GetByIdAsync(jobId, cancellationToken);
        if (job is null)
        {
            return null;
        }

        var errors = string.IsNullOrWhiteSpace(job.ErrorDetails)
            ? []
            : JsonSerializer.Deserialize<List<ImportRowError>>(job.ErrorDetails) ?? [];

        return new ImportJobResult
        {
            JobId = job.Id,
            Status = job.Status,
            TotalRows = job.TotalRows,
            SuccessRows = job.SuccessRows,
            FailedRows = job.FailedRows,
            Errors = errors
        };
    }

    private async Task<ImportJobResult> FailAsync(Guid jobId, string message, CancellationToken cancellationToken)
    {
        var errors = new List<ImportRowError> { new() { Row = 0, Column = "", Message = message } };
        await importJobRepository.CompleteAsync(jobId, "Failed", 0, 0, 0, JsonSerializer.Serialize(errors), cancellationToken);
        return new ImportJobResult { JobId = jobId, Status = "Failed", Errors = errors };
    }
}
