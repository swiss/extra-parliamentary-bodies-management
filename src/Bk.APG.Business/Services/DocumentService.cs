using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Bk.APG.CrossCutting.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bk.APG.Business.Services;

public class DocumentService : IDocumentService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Configuration _s3Configuration;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService([FromKeyedServices("apg")] IAmazonS3 s3Client, IOptions<S3Configuration> s3Configuration, ILogger<DocumentService> logger)
    {
        _s3Client = s3Client;
        _s3Configuration = s3Configuration.Value;
        _logger = logger;
    }

    public async Task<string> UploadDocument(byte[] fileContentBytes)
    {
        var key = Guid.NewGuid().ToString();
        _logger.LogInformation("Upload document with key {Key}", key);

        using var stream = new MemoryStream(fileContentBytes);

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = stream,
            BucketName = _s3Configuration.bucket,
            Key = key,
            ContentType = "application/pdf"
        };

        using var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(uploadRequest);

        _logger.LogInformation("Uploaded new document with key {Key}", key);

        return key;
    }

    public async Task SetupStorage()
    {
        var bucketName = _s3Configuration.bucket;

        _logger.LogInformation("Setting up S3 bucket: {Bucket}", bucketName);

        var listBucketResponse = await _s3Client.ListBucketsAsync();
        if (listBucketResponse.Buckets is not null && listBucketResponse.Buckets.Any(x => x.BucketName == bucketName))
        {
            _logger.LogInformation("Bucket {Bucket} already exists, emptying it.", bucketName);

            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
            };

            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                foreach (var s3Object in response.S3Objects)
                {
                    await _s3Client.DeleteObjectAsync(bucketName, s3Object.Key);
                }

                // If the response is truncated, set the request ContinuationToken
                // from the NextContinuationToken property of the response.
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);
        }
        else
        {
            _logger.LogInformation("Creating bucket {Bucket}", bucketName);
            await _s3Client.PutBucketAsync(bucketName);
        }
    }

    public async Task<MemoryStream?> GetDocument(string documentId)
    {
        try
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = _s3Configuration.bucket,
                Key = documentId
            };

            using var response = await _s3Client.GetObjectAsync(getObjectRequest);
            await using var stream = response.ResponseStream;

            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
        catch (AmazonS3Exception e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            throw;
        }
    }

    public async Task RemoveDocument(string documentId)
    {
        _logger.LogInformation("Delete document with key {Key}", documentId);

        await _s3Client.DeleteAsync(_s3Configuration.bucket, documentId, null);

        _logger.LogInformation("Document with key {Key} has been deleted", documentId);
    }
}
