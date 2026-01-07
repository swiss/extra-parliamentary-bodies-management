namespace Bk.APG.Business.Models;

public class Interest : EntityBase
{
    public int OgdId { get; set; }
    public string? Text { get; set; }
    // TODO, this field is used to store the value from UID. It will be the required field, once manual completion is done!
    public string? InterestText { get; set; }
    public Person? Person { get; set; }
    public Guid PersonId { get; set; }
    public InterestCommittee? InterestCommittee { get; set; }
    public Guid InterestCommitteeId { get; set; }
    public InterestFunction? InterestFunction { get; set; }
    public Guid InterestFunctionId { get; set; }
    // TODO: the old masterdata has to be removed, when final data migration is done. 
    public InterestLegalForm? InterestLegalForm { get; set; }
    public Guid? InterestLegalFormId { get; set; }
    public LegalForm? LegalForm { get; set; }
    // TODO: after the data migration, this field should be set to required here
    public Guid? LegalFormId { get; set; }
    public DateOnly? BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? UidOrganisationId { get; set; }
    public int OldId { get; set; }
    public bool IsDeleted { get; set; }
    public bool? VerifiedSuccessfully { get; set; }
    // Two fields for migration only, can be deleted, once manual completion is done
    public string? UidOrganisationNameClosestMatch { get; set; }
    public int? UidMatchQuality { get; set; }
    public uint RowVersion { get; set; }
}
