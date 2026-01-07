using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class SparqlOptions
{
    public const string SectionKey = "Sparql";

    public string? Username { get; init; }

    public string? Password { get; init; }

    [Required] public required int RequestTimeoutMs { get; init; }

    [Required] public required string Endpoint { get; init; }

    [Required] public required ProxyOptions MasterDataProxy { get; init; }

    [Required] public required ProxyOptions ExportProxy { get; init; }

    [Required] public required string ExportGraphName { get; init; }

    [Required] public required string ExportGraphBaseUri { get; init; }

    [Required] public required string ExportGraphVersion { get; set; }

    [Required] public required bool ExportEnabled { get; set; }
}

public class ProxyOptions
{
    [Required] public required bool UseProxy { get; init; }

    public string? Address { get; init; }
}
