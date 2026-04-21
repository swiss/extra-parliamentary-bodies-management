using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class OpenDataStackFrontendOptions
{
    [Required]
    public required bool Enabled { get; init; }

    [Required]
    public required string BaseUrl { get; init; }

    [Required]
    public required string Dashboard { get; init; }
}
