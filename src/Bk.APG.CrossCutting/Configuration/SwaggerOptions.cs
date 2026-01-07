using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class SwaggerOptions
{
    public const string SectionKey = "Swagger";

    [Required]
    public required bool Enabled { get; init; }
}
