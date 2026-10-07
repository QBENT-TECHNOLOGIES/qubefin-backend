using FluentValidation;
using Microsoft.AspNetCore.Http;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>The candidate's CV and job application: an image or a PDF, both mandatory at creation.</summary>
public static class CandidateDocuments
{
    public static bool IsImageOrPdf(IFormFile? file) =>
        file is not null &&
        (file.ContentType?.StartsWith("image/", StringComparison.OrdinalIgnoreCase) == true ||
         string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase));

    /// <summary>Required (when creating) or optional (when replacing on update) image/PDF upload rule.</summary>
    public static IRuleBuilderOptions<T, IFormFile?> CandidateDocument<T>(this IRuleBuilder<T, IFormFile?> rule, string name, bool required)
    {
        return rule
            .Must(file => !required || (file is not null && file.Length > 0)).WithMessage($"{name} is required.")
            .Must(file => file is null || IsImageOrPdf(file)).WithMessage($"{name} must be an image or a PDF.");
    }

    /// <summary>Uploads the file and returns its storage key, or null when no file was sent.</summary>
    public static async Task<string?> UploadIfPresentAsync(this IFileStorageRepository fileStorageRepository, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        await using var stream = file.OpenReadStream();
        return await fileStorageRepository.UploadFileAsync(
            stream,
            file.FileName,
            file.ContentType ?? "application/octet-stream",
            cancellationToken);
    }
}
