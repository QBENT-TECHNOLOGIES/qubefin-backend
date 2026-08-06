using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IFileStorageRepository
{
    Task<string> UploadFileAsync(Stream stream, string key, string contentType, CancellationToken cancellationToken = default);
    Task<string> GetFileUrlAsync(string key, CancellationToken cancellationToken = default);
}

public class FileStorageRepository(IConfiguration configuration) : IFileStorageRepository
{
    public async Task<string> UploadFileAsync(Stream stream, string key, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            var uploadName = DateTime.Now.Ticks.ToString() + "_" + key;

            using (var client = new AmazonS3Client($@"{configuration["aws:awsAccessKeyId"]}", $@"{configuration["aws:awsSecretAccessKey"]}", RegionEndpoint.GetBySystemName($@"{configuration["aws:region"]}")))
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = stream,
                        Key = "WeGrow/" + uploadName,
                        BucketName = $@"{configuration["aws:bucket"]}",
                        CannedACL = S3CannedACL.PublicRead
                    };
                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }
            }
            return uploadName;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error uploading file to storage: {ex.Message}", ex);
        }
    }
    public async Task<string> GetFileUrlAsync(string key, CancellationToken cancellationToken = default)
    {
        var s3Client = new AmazonS3Client($@"{configuration["aws:awsAccessKeyId"]}", $@"{configuration["aws:awsSecretAccessKey"]}", RegionEndpoint.GetBySystemName($@"{configuration["aws:region"]}"));
        var request = new GetPreSignedUrlRequest
        {
            BucketName = $@"{configuration["aws:bucket"]}",
            Key = "WeGrow/" + key,
            Expires = DateTime.UtcNow.AddMinutes(30)
        };
        string presignedUrl = s3Client.GetPreSignedURL(request);
        return presignedUrl;
    }
}