using System.ComponentModel.DataAnnotations.Schema;

namespace Bk.APG.Business.Models;

public class ContactPoint : EntityBase
{
    public int OgdId { get; set; }
    public Committee? Committee { get; set; }
    public required Guid CommitteeId { get; set; }
    public ContactPointType? ContactPointType { get; set; }
    public required Guid ContactPointTypeId { get; set; }
    public string? CompanyName { get; set; }
    public string? Section { get; set; }
    public DateOnly BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Street { get; set; }
    public string? PoBox { get; set; }
    public string? Zip { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Surname { get; set; }
    public string? GivenName { get; set; }
    public string? Title { get; set; }
    public Language? Language { get; set; }
    public Guid? LanguageId { get; set; }
    public Gender? Gender { get; set; }
    public Guid? GenderId { get; set; }
    public string? PersonalPhone { get; set; }
    public string? PersonalMobile { get; set; }
    public string? PersonalEmail { get; set; }
    public bool ReleasePersonData { get; set; }
    public bool VerifiedSuccessfully { get; set; }
    public int? VerificationCode { get; set; }
    public uint RowVersion { get; set; }

    public int OldId { get; set; }

    [NotMapped]
    public bool IsFemale => Gender?.Uri == Gender.Female;

    [NotMapped]
    public bool IsDataProtectionOfficer => ContactPointTypeId == ContactPointType.DataProtectionOfficerGuid;
}
