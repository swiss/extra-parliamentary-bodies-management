using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Dtos;

public class ReportGeneralElectionCommitteeDto
{
    // This DTO, even though called GeneralElection, can also be used for normal committees!
    public Guid Id { get; init; }
    public DateOnly BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public required string DescriptionGerman { get; set; }
    public required string DescriptionFrench { get; set; }
    public required string DescriptionItalian { get; set; }
    public required string DescriptionRomansh { get; set; }
    public Committee? Committee { get; set; }
    public Guid CommitteeId { get; init; }
    public Department? Department { get; set; }
    public Guid DepartmentId { get; set; }
    public Office? Office { get; set; }
    public Guid OfficeId { get; set; }
    public CommitteeLevel? CommitteeLevel { get; set; }
    public Guid CommitteeLevelId { get; set; }
    public CommitteeType? CommitteeType { get; set; }
    public Guid CommitteeTypeId { get; set; }
    public LegalForm? LegalForm { get; set; }
    public Guid? LegalFormId { get; set; }
    public string? OldLegalForm { get; set; }
    public string? LegalBase { get; set; }
    public TermOfOffice? TermOfOffice { get; set; }
    public Guid TermOfOfficeId { get; set; }
    public TermOfOfficeDate? TermOfOfficeDate { get; set; }
    public required Guid TermOfOfficeDateId { get; set; }
    public int? MinimalMembers { get; set; }
    public int? MaximalMembers { get; set; }
    public int? VacanciesGeneralElection { get; set; }
    public string? LinkAuthorityWebsite { get; set; }
    public string? RemarksBaseData { get; set; }
    public string? RemarksBaseDataAdmin { get; set; }
    public required bool IsDeleted { get; set; }
    public string? JustificationMembers { get; set; }
    public string? JustificationGenders { get; set; }
    public string? MeasuresGenders { get; set; }
    public string? JustificationLanguages { get; set; }
    public string? MeasuresLanguages { get; set; }
    public bool? ReleaseGeneralElection { get; set; }
    public bool? FederalLawEstablishment { get; set; }
    public bool? MarketOrientated { get; set; }
    public bool? SupervisionDuty { get; set; }
    public bool AdditionalAuthorityMembers { get; set; }
    public bool? FederalInstitution { get; set; }
    public string? LinkHomepageGerman { get; set; }
    public string? LinkHomepageFrench { get; set; }
    public string? LinkHomepageItalian { get; set; }
    public string? LinkHomepageRomansh { get; set; }
    public required bool IsValidated { get; set; }
    public string? SelectionProcedure { get; set; }

    public ICollection<ReportGeneralElectionMembershipDto> Memberships { get; set; } = new List<ReportGeneralElectionMembershipDto>();

    [NotMapped]
    public bool ExtraParliamentaryCommission => CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid || CommitteeTypeId == CommitteeType.AdministrationCommissionGuid;

    public ICollection<ContactPoint> ContactPoints { get; set; } = new List<ContactPoint>();

    public ICollection<MembershipAddition> MembershipAdditions { get; set; } = new List<MembershipAddition>();

    public string GetDescription()
    {
        return GetDescription(CultureInfo.CurrentUICulture);
    }

    public string GetDescription(CultureInfo cultureInfo)
    {
        return cultureInfo.TwoLetterISOLanguageName switch
        {
            Language.German => DescriptionGerman,
            Language.French => !string.IsNullOrWhiteSpace(DescriptionFrench) ? DescriptionFrench : DescriptionGerman,
            Language.Italian => !string.IsNullOrWhiteSpace(DescriptionItalian) ? DescriptionItalian : DescriptionGerman,
            _ => DescriptionGerman
        };
    }

    [NotMapped]
    public int ActiveMemberCount => Memberships.Count(x => x is { IsSelected: true, IsDeleted: false });

    [NotMapped]
    public int FemaleCount => Memberships.Count(x => x is { IsSelected: true, Person.Gender.Uri: Gender.Female, IsDeleted: false });

    [NotMapped]
    public int MaleCount => Memberships.Count(x => x is { IsSelected: true, Person.Gender.Uri: Gender.Male, IsDeleted: false });

    [NotMapped]
    public bool FemaleUnderStuffed => ActiveMemberCount > 0 && 100 / ActiveMemberCount * FemaleCount < CommitteeType!.FemaleThreshold;

    [NotMapped]
    public bool MaleUnderStuffed => ActiveMemberCount > 0 && 100 / ActiveMemberCount * MaleCount < CommitteeType!.MaleThreshold;

    [NotMapped]
    public int GermanCount => Memberships.Count(x => x is { IsSelected: true, Person.Language.Uri: Language.GermanUri, IsDeleted: false });

    [NotMapped]
    public int FrenchCount => Memberships.Count(x => x is { IsSelected: true, Person.Language.Uri: Language.FrenchUri, IsDeleted: false });

    [NotMapped]
    public int ItalianCount => Memberships.Count(x => x is { IsSelected: true, Person.Language.Uri: Language.ItalianUri, IsDeleted: false });

    [NotMapped]
    public int RomanshCount => Memberships.Count(x => x is { IsSelected: true, Person.Language.Uri: Language.RomanshUri, IsDeleted: false });

}
