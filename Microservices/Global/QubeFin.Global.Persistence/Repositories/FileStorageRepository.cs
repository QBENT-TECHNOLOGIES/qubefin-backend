using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace QubeFin.Global.Persistence.Repositories;

public interface IFileStorageRepository
{
    Task<string> UploadFileAsync(Stream stream, string fileName, string folder, CancellationToken cancellationToken = default);
    Task<string> GetFileUrlAsync(string key, string folder);
    Task DeleteFileAsync(string key, string folder, CancellationToken cancellationToken = default);
}

// Same idea as the file storage in the Hrms service, but the files stay PRIVATE:
// nobody can open them by guessing the address. They are opened through a short-lived link.
// The settings (aws:awsAccessKeyId, aws:awsSecretAccessKey, aws:region, aws:bucket) are read from configuration.
public class FileStorageRepository(IConfiguration configuration) : IFileStorageRepository
{
    private const string RootFolder = "WeGrow/";

    // Returns the name the file was stored under (unique number + the original name).
    public async Task<string> UploadFileAsync(Stream stream, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        var storedName = $"{DateTime.Now.Ticks}_{fileName}";

        using var client = CreateClient();
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = stream,
            Key = BuildKey(folder, storedName),
            BucketName = configuration["aws:bucket"]
        };
        await new TransferUtility(client).UploadAsync(uploadRequest, cancellationToken);
        return storedName;
    }

    // A link that works for 30 minutes.
    public Task<string> GetFileUrlAsync(string key, string folder)
    {
        using var client = CreateClient();
        var request = new GetPreSignedUrlRequest
        {
            BucketName = configuration["aws:bucket"],
            Key = BuildKey(folder, key),
            Expires = DateTime.UtcNow.AddMinutes(30)
        };
        return Task.FromResult(client.GetPreSignedURL(request));
    }

    public async Task DeleteFileAsync(string key, string folder, CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();
        await client.DeleteObjectAsync(configuration["aws:bucket"], BuildKey(folder, key), cancellationToken);
    }

    private AmazonS3Client CreateClient()
    {
        return new AmazonS3Client(
            configuration["aws:awsAccessKeyId"],
            configuration["aws:awsSecretAccessKey"],
            RegionEndpoint.GetBySystemName(configuration["aws:region"]));
    }

    private static string BuildKey(string folder, string storedName)
    {
        return $"{RootFolder}{folder}/{storedName}";
    }
}
