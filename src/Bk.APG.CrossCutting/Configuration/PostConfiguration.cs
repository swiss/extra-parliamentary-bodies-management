using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class PostConfiguration
{
    public const string SectionKey = "Post";

    [Required]
    public required string Uri { get; init; }
    [Required]
    public required string Username { get; init; }
    [Required]
    public required string Password { get; init; }
}
