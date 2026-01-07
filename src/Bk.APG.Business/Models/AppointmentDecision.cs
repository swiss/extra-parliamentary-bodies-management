using System.ComponentModel.DataAnnotations.Schema;

namespace Bk.APG.Business.Models;

public class AppointmentDecision : EntityBase
{
    public int OgdId { get; set; }
    public Committee? Committee { get; set; }
    public required Guid CommitteeId { get; set; }
    public AppointmentDecisionType? AppointmentDecisionType { get; set; }
    public Guid? AppointmentDecisionTypeId { get; set; }
    public AppointmentDecisionLinkType? AppointmentDecisionLinkType { get; set; }
    public Guid? AppointmentDecisionLinkTypeId { get; set; }
    public DateOnly AppointmentDecisionDate { get; set; }
    public string? Text { get; set; }
    public string? Link { get; set; }
    public DocumentStorage? OriginalDocument { get; set; }
    public Guid? OriginalDocumentId { get; set; }
    public DocumentStorage? FileReferenceGerman { get; set; }
    public Guid? FileReferenceGermanId { get; set; }
    public DocumentStorage? FileReferenceFrench { get; set; }
    public Guid? FileReferenceFrenchId { get; set; }
    public DocumentStorage? FileReferenceItalian { get; set; }
    public Guid? FileReferenceItalianId { get; set; }
    public DocumentStorage? FileReferenceRomansh { get; set; }
    public Guid? FileReferenceRomanshId { get; set; }
    public uint RowVersion { get; set; }
    public int OldId { get; set; }

    [NotMapped]
    public int FileReferenceCount
    {
        get
        {
            var count = 0;

            if (FileReferenceGerman is not null)
            {
                count++;
            }

            if (FileReferenceFrench is not null)
            {
                count++;
            }

            if (FileReferenceItalian is not null)
            {
                count++;
            }

            if (FileReferenceRomansh is not null)
            {
                count++;
            }

            return count;
        }
    }
}
