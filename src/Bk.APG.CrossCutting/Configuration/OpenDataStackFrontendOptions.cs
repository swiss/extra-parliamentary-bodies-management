using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class OpenDataStackFrontendOptions
{
    [Required]
    public required string BaseUrl { get; init; }

    [Required]
    public required string InitialDashboardId { get; init; }
}
