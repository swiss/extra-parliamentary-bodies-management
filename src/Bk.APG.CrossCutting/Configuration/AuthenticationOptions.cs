using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class AuthenticationOptions
{
    public const string SectionKey = "Authentication";

    [Required]
    public required string Url { get; init; }

    [Required]
    public required string ClientId { get; init; }

    [Required]
    public required bool IsHttps { get; init; }
}
