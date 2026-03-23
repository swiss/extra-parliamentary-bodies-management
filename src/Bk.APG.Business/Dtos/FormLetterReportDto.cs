namespace Bk.APG.Business.Dtos;

public class FormLetterReportDto
{
    public string? SenderOffice { get; set; }
    public string? SenderOfficeShort { get; set; }
    public string? SenderName { get; set; }
    public string? SenderStreet { get; set; }
    public string? SenderZip { get; set; }
    public string? SenderCity { get; set; }
    public string? SenderPhone { get; set; }
    public string? SenderEmail { get; set; }
    public string? SenderWebsite { get; set; }
    public string? SenderSignature { get; set; }
    public bool HasSignature { get; set; }
    public string? NextTermOfOfficeBeginDate { get; set; }
    public string? NextTermOfOfficeEndDate { get; set; }
    public string? TermOfOfficeEndDate { get; set; }

    public IEnumerable<FormLetterMembershipReportDto>? Memberships { get; set; }
}

public class FormLetterMembershipReportDto
{
    public string? SenderDepartment { get; set; }
    public string? SenderOffice { get; set; }
    public string? SenderOfficeShort { get; set; }
    public string? SenderFunction { get; set; }
    public string? SenderName { get; set; }
    public string? SenderStreet { get; set; }
    public string? SenderZip { get; set; }
    public string? SenderCity { get; set; }

    public string? Subject { get; set; }
    public string? DateLetter { get; set; }
    public required FormLetterType FormLetterType { get; set; }
    public required FormLetterLanguage FormLetterLanguage { get; set; }
    public required Guid CommitteeId { get; set; }
    public required string CommitteeName { get; set; }
    public required string Function { get; set; }
    public string? Salutation { get; set; }
    public string? SalutationText { get; set; }
    public required string GivenName { get; set; }
    public required string Surname { get; set; }
    public required Guid CorrespondenceLanguageId { get; set; }
    public string? CompanyName { get; set; }
    public string? Street { get; set; }
    public string? PoBox { get; set; }
    public string? Zip { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}

public enum FormLetterType
{
    NewElection,
    ReElection,
    Retire,
    MaximumMembershipDuration,
    OtherRetirement
}

public enum FormLetterLanguage
{
    German,
    French,
    Italian,
    Romansh
}
