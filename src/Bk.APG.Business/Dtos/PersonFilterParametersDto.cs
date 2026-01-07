namespace Bk.APG.Business.Dtos;

public class PersonFilterParametersDto
{
    public string? FreeText { get; set; }
    public IEnumerable<bool>? HasActiveMembership { get; set; }
    public IEnumerable<Guid>? CantonIds { get; set; }
    public IEnumerable<Guid>? LanguageIds { get; set; }
}
