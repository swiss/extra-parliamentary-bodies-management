namespace Bk.APG.Business.Models;

public class FormLetterSender : EntityBase
{
    public required string Description { get; set; }
    public required string Surname { get; set; }
    public required string GivenName { get; set; }
    public FormLetterSenderFunction? SenderFunction { get; set; }
    public Guid SenderFunctionId { get; set; }
    public Department? Department { get; set; }
    public Guid DepartmentId { get; set; }
    public Office? Office { get; set; }
    public Guid OfficeId { get; set; }
    public required string StreetGerman { get; set; }
    public required string StreetFrench { get; set; }
    public required string StreetItalian { get; set; }
    public required string StreetRomansh { get; set; }
    public required string Zip { get; set; }
    public required string CityGerman { get; set; }
    public required string CityFrench { get; set; }
    public required string CityItalian { get; set; }
    public required string CityRomansh { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public DocumentStorage? SignatureFileReference { get; set; }
    public Guid? SignatureFileReferenceId { get; set; }
    public uint RowVersion { get; set; }
}
