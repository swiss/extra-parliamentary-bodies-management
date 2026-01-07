using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class OgdS3Configuration
{
    public const string SectionKey = "OgdS3";

    [Required]
    public required string s3_endpoint { get; init; }

    [Required]
    public required string access_key { get; init; }

    [Required]
    public required string secret_access_key { get; init; }

    [Required]
    public required string bucket { get; init; }

    [Required]
    public required string BaseUrl { get; init; }
}
