using Bk.APG.Business.Models;

namespace Bk.APG.Business.Dtos;

public class ReportGeneralElectionMembershipDto
{
    public Guid Id { get; set; }
    public Person? Person { get; set; }
    public Guid? PersonId { get; set; }

    // Life might be easier, if we have all the person fields on the report dto
    public required string Surname { get; set; }
    public required string GivenName { get; set; }
    public required int BirthYear { get; set; }
    public Guid LanguageId { get; set; }
    public Guid GenderId { get; set; }
    public int? MaximumEmploymentLevel { get; set; }
    public DateOnly BeginDate { get; set; }
    public DateOnly EndDate { get; set; }
    public ElectionType? ElectionType { get; set; }
    public Guid ElectionTypeId { get; set; }
    public Function? Function { get; set; }
    public Guid FunctionId { get; set; }
    public ElectionOffice? ElectionOffice { get; set; }
    public Guid ElectionOfficeId { get; set; }
    public string? OldMembershipAddition { get; set; }
    public Guid? MembershipAdditionId { get; set; }
    public string? JustificationLongerDuty { get; set; }
    public string? JustificationShorterDuty { get; set; }
    public string? JustificationMemberInFederalDuty { get; set; }
    public string? JustificationMemberInFederalAssembly { get; set; }
    public string? RequirementsProfile { get; set; }
    public string? Remarks { get; set; }
    public string? RemarksStatus { get; set; }
    public required bool InCorrelationWithFederalDuty { get; set; }
    public required bool IsDeleted { get; set; }
    public required bool IsSelected { get; set; }
}
