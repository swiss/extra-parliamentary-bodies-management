using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class AuthorizationOptions
{
    public const string SectionKey = "Authorization";

    [Required]
    public required string Allow { get; set; }

    [Required]
    public required string Admin { get; set; }

    [Required]
    public required string Department { get; set; }

    [Required]
    public required string Office { get; set; }

    [Required]
    public required string Secretariat { get; set; }

    [Required]
    public required string Observer { get; set; }
}
