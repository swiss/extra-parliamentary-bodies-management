namespace Bk.APG.Business.Dtos;

public class AppointmentDecisionListDto
{
    public required Guid Id { get; set; }
    public DateOnly AppointmentDecisionDate { get; set; }
    public string? AppointmentDecisionType { get; set; }
    public string? AppointmentDecisionLinkType { get; set; }
    public string? DocumentStorageId { get; set; }
    public string? Text { get; set; }
    public string? LinkText { get; set; }
    public string? Link { get; set; }
    public string? FileName { get; set; }
    public DateTime Modified { get; set; }
}
