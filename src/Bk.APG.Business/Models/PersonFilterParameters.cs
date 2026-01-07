namespace Bk.APG.Business.Models;

public class PersonFilterParameters
{
    public string? FreeText { get; set; }
    public IEnumerable<bool>? HasActiveMembership { get; set; }
    public IEnumerable<Guid>? CantonIds { get; set; }
    public IEnumerable<Guid>? LanguageIds { get; set; }

}
