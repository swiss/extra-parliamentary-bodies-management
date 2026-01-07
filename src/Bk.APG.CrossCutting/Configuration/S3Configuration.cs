using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class S3Configuration
{
    public const string SectionKey = "S3";

    [Required]
    public required string s3_endpoint { get; init; }

    [Required]
    public required string access_key { get; init; }

    [Required]
    public required string secret_access_key { get; init; }

    [Required]
    public required string bucket { get; init; }
}
