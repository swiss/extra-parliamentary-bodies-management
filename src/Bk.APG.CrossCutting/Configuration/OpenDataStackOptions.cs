using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class OpenDataStackOptions
{
    public const string SectionKey = "OpenDataStack";

    [Required]
    public required string BaseUrl { get; init; }

    [Required]
    public required string TokenEndpoint { get; init; }
}
