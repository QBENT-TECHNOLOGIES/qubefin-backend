using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface IFileStorageRepository
{
    Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);
    string? GetFileUrl(string? key);
}

// Same bucket and settings as HRMS (aws:awsAccessKeyId, aws:awsSecretAccessKey, aws:region, aws:bucket), and the same naming
// as the HRMS leave request upload: the stored name is "{ticks}_{file name}", e.g. "639304524813277536_photo_2.jpg", and the
// file is at WeGrow/Tracking/{stored name}. Only the stored name goes in the database (Tbl_BCGroup / Tbl_BCMember /
// Tbl_Employee.LocationPhoto, Tbl_JobTask.PhotoUrl); the GET APIs turn it into a signed link with GetFileUrl.
public class FileStorageRepository(IConfiguration configuration) : IFileStorageRepository
{
    private const string Root = "WeGrow/";
    private const string Folder = "Tracking/";

    // LocationPhoto holds 50 characters: a longer file name is shortened, keeping its extension
    private const int MaxStoredName = 50;

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var storedName = StoredNameOf(fileName);
        using var client = CreateClient();
        await new TransferUtility(client).UploadAsync(new TransferUtilityUploadRequest
        {
            InputStream = stream,
            Key = Root + Folder + storedName,
            BucketName = configuration["aws:bucket"],
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        }, cancellationToken);
        return storedName;
    }

    // A signed link the web can open directly in an image viewer for 30 minutes.
    // A full URL is sent as it is; a value with a folder ("Tracking/Groups/…", saved before this naming) is kept under WeGrow/.
    public string? GetFileUrl(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        if (key.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || key.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return key;

        using var client = CreateClient();
        return client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = configuration["aws:bucket"],
            Key = Root + (key.Contains('/') ? key : Folder + key),
            Expires = DateTime.UtcNow.AddMinutes(30)
        });
    }

    // "{ticks}_{file name}", e.g. 639304524813277536_photo_2.jpg; the name part is cut (extension kept) to fit 50 characters
    private static string StoredNameOf(string fileName)
    {
        var name = Path.GetFileName(fileName);
        if (string.IsNullOrWhiteSpace(name)) name = "photo.jpg";
        var prefix = $"{DateTime.Now.Ticks}_";
        if (prefix.Length + name.Length <= MaxStoredName) return prefix + name;

        var extension = Path.GetExtension(name);
        if (extension.Length > 10) extension = string.Empty;
        var room = Math.Max(1, MaxStoredName - prefix.Length - extension.Length);
        var stem = Path.GetFileNameWithoutExtension(name);
        return prefix + stem[..Math.Min(stem.Length, room)] + extension;
    }

    private AmazonS3Client CreateClient() =>
        new(configuration["aws:awsAccessKeyId"], configuration["aws:awsSecretAccessKey"], RegionEndpoint.GetBySystemName(configuration["aws:region"]));
}
