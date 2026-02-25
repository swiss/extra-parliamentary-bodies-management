using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class SparqlOptions
{
    public const string SectionKey = "Sparql";

    [Required]
    public required ProxyOptions MasterDataProxy { get; init; }

    [Required]
    public required string ExportGraphBaseUri { get; init; }

    [Required]
    public required string ExportGraphVersion { get; set; }
}

public class ProxyOptions
{
    [Required]
    public required bool UseProxy { get; init; }

    public string? Address { get; init; }
}
