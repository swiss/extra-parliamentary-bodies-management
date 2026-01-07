using System.ComponentModel.DataAnnotations;

namespace Bk.APG.Infrastructure.Service.UID.Configuration;

public class UidConfiguration
{
    public const string SectionKey = "UID";

    [Required]
    public required string Url { get; init; }

    [Required]
    public required int MinimalMatchQuality { get; init; }

    [Required]
    public required string Username { get; init; }

    [Required]
    public required string Password { get; init; }
}
