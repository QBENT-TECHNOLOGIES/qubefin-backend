using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface IFileStorageRepository
{
    Task<string> UploadFileAsync(Stream stream, string folder, string fileName, string contentType, CancellationToken cancellationToken = default);
    string? GetFileUrl(string? key);
}

// Same bucket and settings as HRMS (aws:awsAccessKeyId, aws:awsSecretAccessKey, aws:region, aws:bucket), under WeGrow/.
// The returned key keeps its folder, e.g. "Tracking/Groups/638982374839283748.jpg", so the URL can be built from it later;
// it stays short because LocationPhoto holds 50 characters.
public class FileStorageRepository(IConfiguration configuration) : IFileStorageRepository
{
    private const string Root = "WeGrow/";

    public async Task<string> UploadFileAsync(Stream stream, string folder, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var key = $"{folder.Trim('/')}/{DateTime.Now.Ticks}{Path.GetExtension(fileName).ToLowerInvariant()}";
        using var client = CreateClient();
        await new TransferUtility(client).UploadAsync(new TransferUtilityUploadRequest
        {
            InputStream = stream,
            Key = Root + key,
            BucketName = configuration["aws:bucket"],
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        }, cancellationToken);
        return key;
    }

    // A signed link the web can open directly in an image viewer for 30 minutes
    public string? GetFileUrl(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        using var client = CreateClient();
        return client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = configuration["aws:bucket"],
            Key = Root + key,
            Expires = DateTime.UtcNow.AddMinutes(30)
        });
    }

    private AmazonS3Client CreateClient() =>
        new(configuration["aws:awsAccessKeyId"], configuration["aws:awsSecretAccessKey"], RegionEndpoint.GetBySystemName(configuration["aws:region"]));
}
