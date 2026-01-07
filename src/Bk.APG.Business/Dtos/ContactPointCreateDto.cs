namespace Bk.APG.Business.Dtos;

public class ContactPointCreateDto
{
    public required Guid CommitteeId { get; set; }
    public required string ContactPointTypeUri { get; set; }
    public required Guid ContactPointTypeId { get; set; }
    public string? CompanyName { get; set; }
    public string? Section { get; set; }
    public DateOnly BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Street { get; set; }
    public string? PoBox { get; set; }
    public required string Zip { get; set; }
    public required string City { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Surname { get; set; }
    public string? GivenName { get; set; }
    public string? Title { get; set; }
    public Guid? LanguageId { get; set; }
    public Guid? GenderId { get; set; }
    public string? PersonalPhone { get; set; }
    public string? PersonalMobile { get; set; }
    public string? PersonalEmail { get; set; }
    public bool ReleasePersonData { get; set; }

    public int OldId { get; set; }
    public bool IsCopy { get; set; }
    public DateOnly CommitteeBeginDate { get; set; }
}
