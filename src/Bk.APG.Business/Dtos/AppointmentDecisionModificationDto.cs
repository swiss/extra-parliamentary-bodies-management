namespace Bk.APG.Business.Dtos;

public class AppointmentDecisionModificationDto
{
    public Guid? AppointmentDecisionTypeId { get; set; }
    public Guid? AppointmentDecisionLinkTypeId { get; set; }
    public DateOnly AppointmentDecisionDate { get; set; }
    public string? Text { get; set; }
    public string? Link { get; set; }
    public IList<DocumentStorageModificationDto>? Documents { get; set; }
}
