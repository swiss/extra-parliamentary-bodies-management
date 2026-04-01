using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Bk.APG.CrossCutting.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bk.APG.Business.Services;

public class OgdDocumentService : IOgdDocumentService
{
    private readonly IAmazonS3 _s3Client;
    private readonly OgdS3Configuration _s3Configuration;
    private readonly ILogger<DocumentService> _logger;

    public OgdDocumentService([FromKeyedServices("ogd")] IAmazonS3 s3Client, IOptions<OgdS3Configuration> s3Configuration, ILogger<DocumentService> logger)
    {
        ArgumentNullException.ThrowIfNull(s3Configuration);

        _s3Client = s3Client;
        _s3Configuration = s3Configuration.Value;
        _logger = logger;
    }

    public async Task UploadDocument(string path, string fileName, Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        _logger.LogInformation("Upload new OGD document {FileName} to S3 bucket {Bucket} at path {Path}", fileName, _s3Configuration.bucket, path);

        // Sanitize filename to prevent header injection
        var safeFileName = fileName.Replace("\r", "", StringComparison.InvariantCultureIgnoreCase).Replace("\n", "", StringComparison.InvariantCultureIgnoreCase);

        // Create ASCII-only version for legacy filename parameter (replace non-ASCII with underscore)
        var asciiFileName = string.Concat(safeFileName.Select(c => c > 127 ? '_' : c)).Replace("\"", "\\\"", StringComparison.InvariantCultureIgnoreCase);

        // UTF-8 percent-encode for RFC 5987/6266 compatibility with non-ASCII characters
        var encodedUtf8FileName = Uri.EscapeDataString(safeFileName);

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = fileStream,
            BucketName = _s3Configuration.bucket,
            Key = $"{path}/{fileName}",
            CannedACL = S3CannedACL.PublicRead,
            Headers =
            {
                ContentDisposition = $"attachment; filename=\"{asciiFileName}\"; filename*=UTF-8''{encodedUtf8FileName}"
            }
        };

        using var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(uploadRequest);

        _logger.LogInformation("Uploaded new OGD document with key {Key}", uploadRequest.Key);
    }

    public async Task SetupBucket()
    {
        if (await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _s3Configuration.bucket))
        {
            await EmptyBucketAsync(_s3Configuration.bucket);
        }
        else
        {
            await _s3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = _s3Configuration.bucket,
                CannedACL = S3CannedACL.PublicRead
            });

            var anonymousAccessPolicy = new
            {
                Version = "2012-10-17",
                Statement = new object[]
                {
                    new
                    {
                        Sid = "PublicRead",
                        Effect = "Allow",
                        Principal = "*",
                        Action = new[] { "s3:GetObject" },
                        Resource = $"arn:aws:s3:::{_s3Configuration.bucket}/*"
                    }
                }
            };

            await _s3Client.PutBucketPolicyAsync(new PutBucketPolicyRequest
            {
                BucketName = _s3Configuration.bucket,
                Policy = JsonSerializer.Serialize(anonymousAccessPolicy)
            });
        }
    }

    private async Task EmptyBucketAsync(string bucketName)
    {
        string? continuationToken = null;
        do
        {
            var listResponse = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = bucketName,
                ContinuationToken = continuationToken
            });

            if (listResponse.S3Objects.Count == 0)
            {
                break;
            }

            var deleteRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName,
                Objects = listResponse.S3Objects.Select(x => new KeyVersion { Key = x.Key }).ToList()
            };

            await _s3Client.DeleteObjectsAsync(deleteRequest);
            continuationToken = listResponse.IsTruncated ? listResponse.NextContinuationToken : null;
        } while (continuationToken != null);
    }
}
